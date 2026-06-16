using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TechMoves_Formative2.Models;
using TechMoves_Formative2.Observers;

namespace TechMoves_Formative2.Controllers
{
    [Authorize]
    public class ContractController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ContractController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient GetClient()
        {
            var client = _httpClientFactory.CreateClient("TechMovesAPI");
            var token = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private async Task PopulateDropDowns(object? selectedClient = null, object? selectedStatus = null)
        {
            var client = GetClient();
            var response = await client.GetAsync("/api/Clients");
            var clients = new List<Client>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                clients = JsonSerializer.Deserialize<List<Client>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<Client>();
            }

            ViewData["ClientID"] = new SelectList(clients, "ClientID", "ClientName", selectedClient);
            ViewData["StatusList"] = new SelectList(new List<string>
            {
                "Draft", "Active", "Expired", "On Hold"
            }, selectedStatus);
        }

        private void NotifyContractObservers(Contract contract)
        {
            var observers = new List<IContractObserver>
            {
                new AdminObserver(),
                new FinanceObserver(),
                new NotificationObserver()
            };

            foreach (var observer in observers)
                observer.Update(contract);
        }

        private async Task<string?> SaveAgreementFile(IFormFile? agreementFile)
        {
            if (agreementFile == null || agreementFile.Length == 0)
                return null;

            var extension = Path.GetExtension(agreementFile.FileName).ToLower();
            if (extension != ".pdf")
                return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "contracts");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await agreementFile.CopyToAsync(stream);

            return "/contracts/" + uniqueFileName;
        }

        // GET: Contract
        public async Task<IActionResult> Index(string statusFilter, DateTime? startDateFilter, DateTime? endDateFilter)
        {
            var client = GetClient();
            var response = await client.GetAsync("/api/Contracts");
            var contracts = new List<Contract>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                contracts = JsonSerializer.Deserialize<List<Contract>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<Contract>();
            }

            if (!string.IsNullOrWhiteSpace(statusFilter))
                contracts = contracts.Where(c => c.Status == statusFilter).ToList();

            if (startDateFilter.HasValue)
                contracts = contracts.Where(c => c.StartDate >= startDateFilter.Value).ToList();

            if (endDateFilter.HasValue)
                contracts = contracts.Where(c => c.EndDate <= endDateFilter.Value).ToList();

            ViewData["StatusFilter"] = statusFilter;
            ViewData["StartDateFilter"] = startDateFilter?.ToString("yyyy-MM-dd");
            ViewData["EndDateFilter"] = endDateFilter?.ToString("yyyy-MM-dd");
            ViewData["StatusList"] = new SelectList(new List<string>
            {
                "Draft", "Active", "Expired", "On Hold"
            }, statusFilter);

            return View(contracts);
        }

        // GET: Contract/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"/api/Contracts/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var contract = JsonSerializer.Deserialize<Contract>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return View(contract);
            }

            return NotFound();
        }

        // GET: Contract/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropDowns();
            return View();
        }

        // POST: Contract/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract)
        {
            if (contract.AgreementFile == null)
                ModelState.AddModelError("AgreementFile", "Please upload a signed agreement PDF.");
            else if (Path.GetExtension(contract.AgreementFile.FileName).ToLower() != ".pdf")
                ModelState.AddModelError("AgreementFile", "Only PDF files are allowed.");

            if (!ModelState.IsValid)
            {
                await PopulateDropDowns(contract.ClientID, contract.Status);
                return View(contract);
            }

            contract.AgreementFilePath = await SaveAgreementFile(contract.AgreementFile);

            var client = GetClient();
            var payload = new StringContent(
                JsonSerializer.Serialize(contract),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("/api/Contracts", payload);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Failed to create contract.");
            await PopulateDropDowns(contract.ClientID, contract.Status);
            return View(contract);
        }

        // GET: Contract/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"/api/Contracts/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var contract = JsonSerializer.Deserialize<Contract>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                await PopulateDropDowns(contract?.ClientID, contract?.Status);
                return View(contract);
            }

            return NotFound();
        }

        // POST: Contract/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract)
        {
            if (contract.AgreementFile != null &&
                Path.GetExtension(contract.AgreementFile.FileName).ToLower() != ".pdf")
                ModelState.AddModelError("AgreementFile", "Only PDF files are allowed.");

            if (!ModelState.IsValid)
            {
                await PopulateDropDowns(contract.ClientID, contract.Status);
                return View(contract);
            }

            // Get existing contract to check status change
            var client = GetClient();
            var existingResponse = await client.GetAsync($"/api/Contracts/{id}");
            if (existingResponse.IsSuccessStatusCode)
            {
                var existingJson = await existingResponse.Content.ReadAsStringAsync();
                var existingContract = JsonSerializer.Deserialize<Contract>(existingJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (existingContract != null && existingContract.Status != contract.Status)
                    NotifyContractObservers(contract);
            }

            if (contract.AgreementFile != null)
                contract.AgreementFilePath = await SaveAgreementFile(contract.AgreementFile);

            var payload = new StringContent(
                JsonSerializer.Serialize(contract),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PutAsync($"/api/Contracts/{id}", payload);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Failed to update contract.");
            await PopulateDropDowns(contract.ClientID, contract.Status);
            return View(contract);
        }

        // GET: Contract/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"/api/Contracts/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var contract = JsonSerializer.Deserialize<Contract>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return View(contract);
            }

            return NotFound();
        }

        // POST: Contract/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = GetClient();
            await client.DeleteAsync($"/api/Contracts/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
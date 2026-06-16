using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TechMoves_Formative2.Factories;
using TechMoves_Formative2.Models;
using TechMoves_Formative2.Services;
using TechMoves_Formative2.Strategies;

namespace TechMoves_Formative2.Controllers
{
    [Authorize]
    public class ServiceRequestController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceRequestFactory _factory;
        private readonly ICurrencyConversionService _currencyConversionService;

        public ServiceRequestController(
            IHttpClientFactory httpClientFactory,
            ICurrencyConversionService currencyConversionService)
        {
            _httpClientFactory = httpClientFactory;
            _factory = new ServiceRequestFactory();
            _currencyConversionService = currencyConversionService;
        }

        private HttpClient GetClient()
        {
            var client = _httpClientFactory.CreateClient("TechMovesAPI");
            var token = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private async Task PopulateDropDowns(object? selectedContract = null, object? selectedStatus = null, object? selectedRequestType = null)
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

            ViewData["ContractID"] = new SelectList(contracts, "ContractID", "ContractName", selectedContract);
            ViewData["StatusList"] = new SelectList(new List<string>
            {
                "Pending", "In Progress", "Completed"
            }, selectedStatus);
            ViewData["RequestTypeList"] = new SelectList(new List<string>
            {
                "Transport", "Delivery"
            }, selectedRequestType);
        }

        // GET: ServiceRequest
        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var response = await client.GetAsync("/api/ServiceRequests");
            var serviceRequests = new List<ServiceRequest>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                serviceRequests = JsonSerializer.Deserialize<List<ServiceRequest>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<ServiceRequest>();
            }

            return View(serviceRequests);
        }

        // GET: ServiceRequest/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"/api/ServiceRequests/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var serviceRequest = JsonSerializer.Deserialize<ServiceRequest>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return View(serviceRequest);
            }

            return NotFound();
        }

        // GET: ServiceRequest/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropDowns();
            return View();
        }

        // POST: ServiceRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest serviceRequest)
        {
            // Get the contract to validate and apply pricing
            var client = GetClient();
            var contractResponse = await client.GetAsync($"/api/Contracts/{serviceRequest.ContractID}");
            Contract? contract = null;

            if (contractResponse.IsSuccessStatusCode)
            {
                var contractJson = await contractResponse.Content.ReadAsStringAsync();
                contract = JsonSerializer.Deserialize<Contract>(contractJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            if (contract == null)
                ModelState.AddModelError("ContractID", "Selected contract does not exist.");
            else if (contract.Status == "Expired" || contract.Status == "On Hold")
                ModelState.AddModelError("", "Cannot create a service request for a contract that is Expired or On Hold.");

            if (!ModelState.IsValid)
            {
                await PopulateDropDowns(serviceRequest.ContractID, serviceRequest.Status, serviceRequest.RequestType);
                return View(serviceRequest);
            }

            // Apply factory and pricing strategy
            var createdRequest = _factory.CreateRequest(serviceRequest.RequestType);
            createdRequest.Description = serviceRequest.Description;
            createdRequest.ContractID = serviceRequest.ContractID;
            createdRequest.Status = serviceRequest.Status;

            IPricingStrategy strategy = contract?.ServiceLevel == "Premium"
                ? new DiscountPricingStrategy()
                : new StandardPricingStrategy();

            createdRequest.Cost = strategy.CalculateCost(serviceRequest.Cost);
            createdRequest.LocalCostZAR = await _currencyConversionService.ConvertUsdToZarAsync(createdRequest.Cost);

            var payload = new StringContent(
                JsonSerializer.Serialize(createdRequest),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("/api/ServiceRequests", payload);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Failed to create service request.");
            await PopulateDropDowns(serviceRequest.ContractID, serviceRequest.Status, serviceRequest.RequestType);
            return View(serviceRequest);
        }

        // GET: ServiceRequest/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"/api/ServiceRequests/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var serviceRequest = JsonSerializer.Deserialize<ServiceRequest>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                await PopulateDropDowns(serviceRequest?.ContractID, serviceRequest?.Status, serviceRequest?.RequestType);
                return View(serviceRequest);
            }

            return NotFound();
        }

        // POST: ServiceRequest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRequest serviceRequest)
        {
            var client = GetClient();
            var contractResponse = await client.GetAsync($"/api/Contracts/{serviceRequest.ContractID}");
            Contract? contract = null;

            if (contractResponse.IsSuccessStatusCode)
            {
                var contractJson = await contractResponse.Content.ReadAsStringAsync();
                contract = JsonSerializer.Deserialize<Contract>(contractJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            if (contract == null)
                ModelState.AddModelError("ContractID", "Selected contract does not exist.");

            if (!ModelState.IsValid)
            {
                await PopulateDropDowns(serviceRequest.ContractID, serviceRequest.Status, serviceRequest.RequestType);
                return View(serviceRequest);
            }

            // Apply pricing strategy
            IPricingStrategy strategy = contract?.ServiceLevel == "Premium"
                ? new DiscountPricingStrategy()
                : new StandardPricingStrategy();

            serviceRequest.Cost = strategy.CalculateCost(serviceRequest.Cost);
            serviceRequest.LocalCostZAR = await _currencyConversionService.ConvertUsdToZarAsync(serviceRequest.Cost);

            var payload = new StringContent(
                JsonSerializer.Serialize(serviceRequest),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PutAsync($"/api/ServiceRequests/{id}", payload);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Failed to update service request.");
            await PopulateDropDowns(serviceRequest.ContractID, serviceRequest.Status, serviceRequest.RequestType);
            return View(serviceRequest);
        }

        // GET: ServiceRequest/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"/api/ServiceRequests/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var serviceRequest = JsonSerializer.Deserialize<ServiceRequest>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return View(serviceRequest);
            }

            return NotFound();
        }

        // POST: ServiceRequest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = GetClient();
            await client.DeleteAsync($"/api/ServiceRequests/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
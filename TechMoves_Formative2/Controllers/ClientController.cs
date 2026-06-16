using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TechMoves_Formative2.Models;

namespace TechMoves_Formative2.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientController(IHttpClientFactory httpClientFactory)
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

        // GET: Client
        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var response = await client.GetAsync("/api/Clients");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var clients = JsonSerializer.Deserialize<List<Client>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return View(clients);
            }

            return View(new List<Client>());
        }

        // GET: Client/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"/api/Clients/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var clientModel = JsonSerializer.Deserialize<Client>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return View(clientModel);
            }

            return NotFound();
        }

        // GET: Client/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Client/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client clientModel)
        {
            if (!ModelState.IsValid)
                return View(clientModel);

            var client = GetClient();
            var payload = new StringContent(
                JsonSerializer.Serialize(clientModel),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("/api/Clients", payload);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Failed to create client.");
            return View(clientModel);
        }

        // GET: Client/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"/api/Clients/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var clientModel = JsonSerializer.Deserialize<Client>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return View(clientModel);
            }

            return NotFound();
        }

        // POST: Client/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client clientModel)
        {
            if (!ModelState.IsValid)
                return View(clientModel);

            var client = GetClient();
            var payload = new StringContent(
                JsonSerializer.Serialize(clientModel),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PutAsync($"/api/Clients/{id}", payload);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Failed to update client.");
            return View(clientModel);
        }

        // GET: Client/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"/api/Clients/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var clientModel = JsonSerializer.Deserialize<Client>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return View(clientModel);
            }

            return NotFound();
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = GetClient();
            var response = await client.DeleteAsync($"/api/Clients/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            return RedirectToAction(nameof(Index));
        }
    }
}
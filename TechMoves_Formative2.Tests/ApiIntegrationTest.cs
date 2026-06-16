using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TechMoves_API;
using TechMoves_API.Data;
using TechMoves_API.DTOs;

namespace TechMoves_Formative2.Tests
{
    public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace SQL Server with in-memory DB for testing
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<TechMoveDb>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<TechMoveDb>(options =>
                        options.UseInMemoryDatabase("TestDb"));
                });
            }).CreateClient();
        }

        // ── AUTH ──────────────────────────────────────────────

        [Fact]
        public async Task Login_ValidCredentials_Returns200WithToken()
        {
            var response = await _client.PostAsJsonAsync("/api/Auth/login", new LoginDto
            {
                Username = "admin",
                Password = "password123"
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
            Assert.NotNull(result);
            Assert.NotEmpty(result!.Token);
        }

        [Fact]
        public async Task Login_InvalidCredentials_Returns401()
        {
            var response = await _client.PostAsJsonAsync("/api/Auth/login", new LoginDto
            {
                Username = "wrong",
                Password = "wrong"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── CLIENTS ───────────────────────────────────────────

        [Fact]
        public async Task GetClients_WithoutToken_Returns401()
        {
            var response = await _client.GetAsync("/api/Clients");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetClients_WithToken_Returns200()
        {
            var token = await GetTokenAsync();
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/Clients");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            Assert.NotNull(json);
        }

        // ── CONTRACTS ─────────────────────────────────────────

        [Fact]
        public async Task GetContracts_WithToken_Returns200()
        {
            var token = await GetTokenAsync();
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/Contracts");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            Assert.NotNull(json);
        }

        [Fact]
        public async Task GetContracts_WithoutToken_Returns401()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync("/api/Contracts");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── SERVICE REQUESTS ──────────────────────────────────

        [Fact]
        public async Task GetServiceRequests_WithToken_Returns200()
        {
            var token = await GetTokenAsync();
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/ServiceRequests");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            Assert.NotNull(json);
        }

        [Fact]
        public async Task GetServiceRequests_WithoutToken_Returns401()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync("/api/ServiceRequests");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── HELPER ────────────────────────────────────────────

        private async Task<string> GetTokenAsync()
        {
            var response = await _client.PostAsJsonAsync("/api/Auth/login", new LoginDto
            {
                Username = "admin",
                Password = "password123"
            });
            var result = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
            return result!.Token;
        }
    }
}
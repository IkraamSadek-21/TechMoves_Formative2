using System.Text.Json;

namespace TechMoves_API.Services
{
        public class CurrencyConversionService : ICurrencyConversionService
        {
            private readonly HttpClient _httpClient;

            public CurrencyConversionService(HttpClient httpClient)
            {
                _httpClient = httpClient;
            }

            public async Task<decimal> ConvertUsdToZarAsync(decimal usdAmount)
            {
                var url = "https://api.frankfurter.dev/v1/latest?base=USD&symbols=ZAR";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(json);

                var rate = document.RootElement
                    .GetProperty("rates")
                    .GetProperty("ZAR")
                    .GetDecimal();

                return Math.Round(usdAmount * rate, 2);
            }
        }
    }


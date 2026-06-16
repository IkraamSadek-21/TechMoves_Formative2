using System.Net;
using System.Text;
using Moq;
using Moq.Protected;
using TechMoves_Formative2.Services;
using Xunit;

namespace TechMoves_Formative2.Tests
{
    public class CurrencyConversionServiceTests
    {
        [Fact]
        public async Task ConvertUsdToZarAsync_ReturnsCorrectConvertedAmount()
        {
            // Arrange
            var fakeJson = """
            {
              "rates": {
                "ZAR": 16.65
              }
            }
            """;

            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(fakeJson, Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var service = new CurrencyConversionService(httpClient);

            // Act
            var result = await service.ConvertUsdToZarAsync(100m);

            // Assert
            Assert.Equal(1665m, result);
        }
    }
}
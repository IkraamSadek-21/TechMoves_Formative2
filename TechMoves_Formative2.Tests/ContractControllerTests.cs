using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using System.Net;
using TechMoves_Formative2.Controllers;
using Xunit;

namespace TechMoves_Formative2.Tests
{
    public class ContractControllerTests
    {
        private ContractController CreateController(HttpResponseMessage? response = null)
        {
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response ?? new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]")
                });

            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            var mockFactory = new Mock<IHttpClientFactory>();
            mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var controller = new ContractController(mockFactory.Object);

            // Fix: mock HttpContext and Session so GetClient() doesn't crash
            var mockSession = new Mock<ISession>();
            byte[]? tokenBytes = null;
            mockSession
                .Setup(s => s.TryGetValue("JwtToken", out tokenBytes))
                .Returns(false);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            return controller;
        }

        [Fact]
        public void ContractController_CanBeInstantiated()
        {
            var controller = CreateController();
            Assert.NotNull(controller);
        }

        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            var controller = CreateController();
            var result = await controller.Index(null, null, null);
            Assert.IsType<ViewResult>(result);
        }
    }
}
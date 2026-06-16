using TechMoves_Formative2.Factories;
using Xunit;

namespace TechMoves_Formative2.Tests
{
    public class ServiceRequestFactoryTests
    {
        [Fact]
        public void CreateRequest_Transport_ReturnsTransportRequest()
        {
            // Arrange
            var factory = new ServiceRequestFactory();

            // Act
            var request = factory.CreateRequest("Transport");

            // Assert
            Assert.NotNull(request);
            Assert.Equal("Transport", request.RequestType);
            Assert.Equal("Pending", request.Status);
        }

        [Fact]
        public void CreateRequest_Delivery_ReturnsDeliveryRequest()
        {
            // Arrange
            var factory = new ServiceRequestFactory();

            // Act
            var request = factory.CreateRequest("Delivery");

            // Assert
            Assert.NotNull(request);
            Assert.Equal("Delivery", request.RequestType);
            Assert.Equal("Pending", request.Status);
        }

        [Fact]
        public void CreateRequest_InvalidType_ThrowsArgumentException()
        {
            // Arrange
            var factory = new ServiceRequestFactory();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => factory.CreateRequest("InvalidType"));
        }
    }
}
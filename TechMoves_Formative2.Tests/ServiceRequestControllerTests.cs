using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using TechMoves_Formative2.Controllers;
using TechMoves_Formative2.Data;
using TechMoves_Formative2.Models;
using TechMoves_Formative2.Services;
using Xunit;

namespace TechMoves_Formative2.Tests
{
    public class ServiceRequestControllerTests
    {
        private TechMoveDb GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TechMoveDb>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new TechMoveDb(options);
        }

        [Fact]
        public async Task Create_WithExpiredContract_AddsModelError()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            var contract = new Contract
            {
                ContractID = 1,
                ContractName = "Expired Contract",
                StartDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(10),
                Status = "Expired",
                ServiceLevel = "Standard",
                ClientID = 1
            };

            context.Contracts.Add(contract);
            context.SaveChanges();

            var currencyServiceMock = new Mock<ICurrencyConversionService>();
            currencyServiceMock
                .Setup(x => x.ConvertUsdToZarAsync(It.IsAny<decimal>()))
                .ReturnsAsync(1800m);

            var controller = new ServiceRequestController(context, currencyServiceMock.Object);

            var request = new ServiceRequest
            {
                RequestType = "Transport",
                Description = "Test request",
                Cost = 100m,
                Status = "Pending",
                ContractID = 1
            };

            // Act
            var result = await controller.Create(request);

            // Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ErrorCount > 0);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_WithOnHoldContract_AddsModelError()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            var contract = new Contract
            {
                ContractID = 2,
                ContractName = "On Hold Contract",
                StartDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(10),
                Status = "On Hold",
                ServiceLevel = "Standard",
                ClientID = 1
            };

            context.Contracts.Add(contract);
            context.SaveChanges();

            var currencyServiceMock = new Mock<ICurrencyConversionService>();
            currencyServiceMock
                .Setup(x => x.ConvertUsdToZarAsync(It.IsAny<decimal>()))
                .ReturnsAsync(1800m);

            var controller = new ServiceRequestController(context, currencyServiceMock.Object);

            var request = new ServiceRequest
            {
                RequestType = "Delivery",
                Description = "Test request",
                Cost = 100m,
                Status = "Pending",
                ContractID = 2
            };

            // Act
            var result = await controller.Create(request);

            // Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ErrorCount > 0);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_WithActiveContract_SavesServiceRequest()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            var contract = new Contract
            {
                ContractID = 3,
                ContractName = "Active Contract",
                StartDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(10),
                Status = "Active",
                ServiceLevel = "Standard",
                ClientID = 1
            };

            context.Contracts.Add(contract);
            context.SaveChanges();

            var currencyServiceMock = new Mock<ICurrencyConversionService>();
            currencyServiceMock
                .Setup(x => x.ConvertUsdToZarAsync(It.IsAny<decimal>()))
                .ReturnsAsync(1800m);

            var controller = new ServiceRequestController(context, currencyServiceMock.Object);

            var request = new ServiceRequest
            {
                RequestType = "Transport",
                Description = "Valid request",
                Cost = 100m,
                Status = "Pending",
                ContractID = 3
            };

            // Act
            var result = await controller.Create(request);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Single(context.ServiceRequests);
        }
    }
}
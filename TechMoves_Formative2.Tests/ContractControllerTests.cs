using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoves_Formative2.Controllers;
using TechMoves_Formative2.Data;
using TechMoves_Formative2.Models;
using Xunit;

namespace TechMoves_Formative2.Tests
{
    public class ContractControllerTests
    {
        private TechMoveDb GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TechMoveDb>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TechMoveDb(options);
        }

        [Fact]
        public async Task Create_WithNonPdfFile_AddsModelError()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Clients.Add(new Client
            {
                ClientID = 1,
                ClientName = "Test Client",
                ClientEmail = "test@test.com",
               
            });
            context.SaveChanges();

            var controller = new ContractController(context);

            var fileContent = new MemoryStream(new byte[] { 1, 2, 3 });
            IFormFile fakeFile = new FormFile(fileContent, 0, fileContent.Length, "AgreementFile", "malware.exe");

            var contract = new Contract
            {
                ContractName = "Test Contract",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30),
                Status = "Active",
                ServiceLevel = "Standard",
                ClientID = 1,
                AgreementFile = fakeFile
            };

            // Act
            var result = await controller.Create(contract);

            // Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey("AgreementFile"));
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public async Task Create_WithNoFile_AddsModelError()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Clients.Add(new Client
            {
                ClientID = 1,
                ClientName = "Test Client",
                ClientEmail = "test@test.com",
               
            });
            context.SaveChanges();

            var controller = new ContractController(context);

            var contract = new Contract
            {
                ContractName = "Test Contract",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30),
                Status = "Active",
                ServiceLevel = "Standard",
                ClientID = 1,
                AgreementFile = null
            };

            // Act
            var result = await controller.Create(contract);

            // Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey("AgreementFile"));
            Assert.IsType<ViewResult>(result);
        }
    }
}
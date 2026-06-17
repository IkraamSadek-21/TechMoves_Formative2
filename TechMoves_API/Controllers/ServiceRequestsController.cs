using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoves_API.Data;
using TechMoves_API.DTOs;
using TechMoves_API.Factories;
using TechMoves_API.Models;
using TechMoves_API.Services;
using TechMoves_API.Strategies;
using static TechMoves_API.DTOs.ServiceRequestDtos;

namespace TechMoves_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly TechMoveDb _context;
        private readonly IServiceRequestFactory _factory;
        private readonly ICurrencyConversionService _currencyService;

        public ServiceRequestsController(TechMoveDb context, IServiceRequestFactory factory, ICurrencyConversionService currencyService)
        {
            _context = context;
            _factory = factory;
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _context.ServiceRequests
                .Select(r => new ServiceRequestResponseDto
                {
                    ServiceRequestID = r.ServiceRequestID,
                    RequestType = r.RequestType,
                    Description = r.Description,
                    Cost = r.Cost,
                    LocalCostZAR = r.LocalCostZAR,
                    Status = r.Status,
                    ContractID = r.ContractID
                }).ToListAsync();

            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var r = await _context.ServiceRequests.FindAsync(id);
            if (r == null) return NotFound();

            return Ok(new ServiceRequestResponseDto
            {
                ServiceRequestID = r.ServiceRequestID,
                RequestType = r.RequestType,
                Description = r.Description,
                Cost = r.Cost,
                LocalCostZAR = r.LocalCostZAR,
                Status = r.Status,
                ContractID = r.ContractID
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServiceRequestDto dto)
        {
            var request = _factory.CreateRequest(dto.RequestType);

            IPricingStrategy strategy = dto.RequestType == "Delivery"
                ? new DiscountPricingStrategy()
                : new StandardPricingStrategy();

            request.Description = dto.Description;
            request.Cost = strategy.CalculateCost(dto.Cost);
            request.Status = dto.Status;
            request.ContractID = dto.ContractID;
            request.LocalCostZAR = await _currencyService.ConvertUsdToZarAsync(request.Cost);

            _context.ServiceRequests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = request.ServiceRequestID }, new ServiceRequestResponseDto
            {
                ServiceRequestID = request.ServiceRequestID,
                RequestType = request.RequestType,
                Description = request.Description,
                Cost = request.Cost,
                LocalCostZAR = request.LocalCostZAR,
                Status = request.Status,
                ContractID = request.ContractID
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateServiceRequestDto dto)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null) return NotFound();

            IPricingStrategy strategy = dto.RequestType == "Delivery"
                ? new DiscountPricingStrategy()
                : new StandardPricingStrategy();

            request.RequestType = dto.RequestType;
            request.Description = dto.Description;
            request.Cost = strategy.CalculateCost(dto.Cost);
            request.Status = dto.Status;
            request.ContractID = dto.ContractID;
            request.LocalCostZAR = await _currencyService.ConvertUsdToZarAsync(request.Cost);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null) return NotFound();

            _context.ServiceRequests.Remove(request);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
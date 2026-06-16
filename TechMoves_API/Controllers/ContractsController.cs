using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TechMoves_API.Data;
using TechMoves_API.DTOs;
using TechMoves_API.Models;
using TechMoves_API.Observers;

namespace TechMoves_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContractsController : ControllerBase
    {
        private readonly TechMoveDb _context;
        private readonly ContractNotifier _notifier;

        public ContractsController(TechMoveDb context, ContractNotifier notifier)
        {
            _context = context;
            _notifier = notifier;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var contracts = await _context.Contracts
                .Include(c => c.Client)
                .Select(c => new ContractResponseDto
                {
                    ContractID = c.ContractID,
                    ContractName = c.ContractName,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Status = c.Status,
                    ServiceLevel = c.ServiceLevel,
                    AgreementFilePath = c.AgreementFilePath,
                    ClientID = c.ClientID,
                    ClientName = c.Client != null ? c.Client.ClientName : string.Empty
                }).ToListAsync();

            return Ok(contracts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var c = await _context.Contracts.Include(c => c.Client).FirstOrDefaultAsync(c => c.ContractID == id);
            if (c == null) return NotFound();

            return Ok(new ContractResponseDto
            {
                ContractID = c.ContractID,
                ContractName = c.ContractName,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Status = c.Status,
                ServiceLevel = c.ServiceLevel,
                AgreementFilePath = c.AgreementFilePath,
                ClientID = c.ClientID,
                ClientName = c.Client?.ClientName ?? string.Empty
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContractDto dto)
        {
            var contract = new Contract
            {
                ContractName = dto.ContractName,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                ServiceLevel = dto.ServiceLevel,
                ClientID = dto.ClientID
            };

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = contract.ContractID }, contract);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateContractStatusDto dto)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return NotFound();

            contract.Status = dto.Status;
            await _context.SaveChangesAsync();

            _notifier.Notify(contract.ContractName, dto.Status);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return NotFound();

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

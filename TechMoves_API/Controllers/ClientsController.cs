using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TechMoves_API.Data;
using TechMoves_API.DTOs;
using TechMoves_API.Models;

namespace TechMoves_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly TechMoveDb _context;

        public ClientsController(TechMoveDb context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clients = await _context.Clients
                .Select(c => new ClientResponseDto
                {
                    ClientID = c.ClientID,
                    ClientName = c.ClientName,
                    ClientEmail = c.ClientEmail,
                    ClientRegion = c.ClientRegion
                }).ToListAsync();

            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            return Ok(new ClientResponseDto
            {
                ClientID = client.ClientID,
                ClientName = client.ClientName,
                ClientEmail = client.ClientEmail,
                ClientRegion = client.ClientRegion
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClientDto dto)
        {
            var client = new Client
            {
                ClientName = dto.ClientName,
                ClientEmail = dto.ClientEmail,
                ClientRegion = dto.ClientRegion
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = client.ClientID }, new ClientResponseDto
            {
                ClientID = client.ClientID,
                ClientName = client.ClientName,
                ClientEmail = client.ClientEmail,
                ClientRegion = client.ClientRegion
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateClientDto dto)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            client.ClientName = dto.ClientName;
            client.ClientEmail = dto.ClientEmail;
            client.ClientRegion = dto.ClientRegion;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

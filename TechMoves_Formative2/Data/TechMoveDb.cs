using Microsoft.EntityFrameworkCore;
using TechMoves_Formative2.Models;

namespace TechMoves_Formative2.Data
{
    public class TechMoveDb : DbContext
    {
        public TechMoveDb(DbContextOptions<TechMoveDb> options) : base(options)
        {
            
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
    }
}

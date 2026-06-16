using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
namespace TechMoves_API.Models
{
    public class Client
    {
        [Key]
        public int ClientID { get; set; }

        [Required]
        public string ClientName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string ClientEmail { get; set; } = string.Empty;

        public string ClientRegion { get; set; } = string.Empty;

        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();


    }
}

using System.ComponentModel.DataAnnotations;

namespace TechMoves_Formative2.Models
{
    public class Client
    {
        [Key]
        [Display(Name = "Client ID")]
        public int ClientID { get; set; }

        [Required]
        [Display(Name = "Client Name")]
        public string ClientName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Client Email")]
        [EmailAddress]
        public string ClientEmail { get; set; } = string.Empty;

        [Display(Name = "Client Region")]
        public string ClientRegion { get; set; } = string.Empty;

        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace TechMoves_API.Models
{
    public class Contract
    {
        [Key]
        public int ContractID { get; set; }

        [Required]
        public string ContractName { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        [Required]
        public string ServiceLevel { get; set; } = string.Empty;

        public string? AgreementFilePath { get; set; }

        public int ClientID { get; set; }

        [JsonIgnore]  
        public Client? Client { get; set; }

        [JsonIgnore]
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();

        //Json ignore prevents an infinite loop that can cause the API to crash, This is used on navigation properties to prevent circular references when serializing objects to JSON.
    }
}

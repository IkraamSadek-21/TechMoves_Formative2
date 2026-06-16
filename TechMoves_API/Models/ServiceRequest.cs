using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace TechMoves_API.Models
{
    public class ServiceRequest
    {
        [Key]
        public int ServiceRequestID { get; set; }

        [Required]
        public string RequestType { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? LocalCostZAR { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public int ContractID { get; set; }

        [JsonIgnore]
        public Contract? Contract { get; set; }
    }
}

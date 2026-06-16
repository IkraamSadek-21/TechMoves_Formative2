using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechMoves_Formative2.Models
{
    public class ServiceRequest
    {
        [Key]
        public int ServiceRequestID { get; set; }

        [Required]
        [Display(Name = "Request Type")]
        public string RequestType { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Cost (USD)")]
        public decimal Cost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Local Cost (ZAR)")]
        public decimal? LocalCostZAR { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "Contract")]
        public int ContractID { get; set; }

        public Contract? Contract { get; set; }
    }
}
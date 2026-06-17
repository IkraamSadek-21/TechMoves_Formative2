using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechMoves_Formative2.Models
{
    public class Contract
    {
        [Key]
        [Display(Name = "Contract ID")]
        public int ContractID { get; set; }

        [Required]
        [Display(Name = "Contract Name")]
        public string ContractName { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        [Display(Name = "Service Level")]
        public string ServiceLevel { get; set; }

        [Display(Name = "Signed Agreement File")]
        public string? AgreementFilePath { get; set; }

        [NotMapped]
        [Display(Name = "Signed Agreement PDF")]
        public IFormFile? AgreementFile { get; set; }

        [Display(Name = "Client")]
        public int ClientID { get; set; }
        public Client? Client { get; set; }

        public string? ClientName { get; set; }

        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}
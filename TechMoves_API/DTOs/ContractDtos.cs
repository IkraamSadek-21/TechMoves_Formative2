namespace TechMoves_API.DTOs
{
    public class CreateContractDto
    {
        public string ContractName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ServiceLevel { get; set; } = string.Empty;
        public int ClientID { get; set; }
    }

    public class UpdateContractStatusDto
    {
        public string Status { get; set; } = string.Empty;
    }

    public class ContractResponseDto
    {
        public int ContractID { get; set; }
        public string ContractName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ServiceLevel { get; set; } = string.Empty;
        public string? AgreementFilePath { get; set; }
        public int ClientID { get; set; }
        public string ClientName { get; set; } = string.Empty;
    }
}

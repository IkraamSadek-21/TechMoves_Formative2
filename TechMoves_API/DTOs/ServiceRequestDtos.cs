namespace TechMoves_API.DTOs
{
    public class ServiceRequestDtos
    {
        public class CreateServiceRequestDto
        {
            public string RequestType { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal Cost { get; set; }
            public string Status { get; set; } = string.Empty;
            public int ContractID { get; set; }
        }

        public class ServiceRequestResponseDto
        {
            public int ServiceRequestID { get; set; }
            public string RequestType { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal Cost { get; set; }
            public decimal? LocalCostZAR { get; set; }
            public string Status { get; set; } = string.Empty;
            public int ContractID { get; set; }
        }
    }
}

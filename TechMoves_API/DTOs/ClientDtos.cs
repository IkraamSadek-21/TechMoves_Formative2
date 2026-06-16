namespace TechMoves_API.DTOs
{
    public class CreateClientDto
    {
        public string ClientName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientRegion { get; set; } = string.Empty;
    }

    public class ClientResponseDto
    {
        public int ClientID { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientRegion { get; set; } = string.Empty;
    }
}

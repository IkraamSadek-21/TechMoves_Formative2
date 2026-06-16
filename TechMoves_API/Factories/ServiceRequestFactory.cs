using TechMoves_API.Models;
namespace TechMoves_API.Factories
{
    public class ServiceRequestFactory : IServiceRequestFactory
    {
        public ServiceRequest CreateRequest(string requestType)
        {
            return requestType switch
            {
                "Transport" => new ServiceRequest { RequestType = "Transport", Status = "Pending" },
                "Delivery" => new ServiceRequest { RequestType = "Delivery", Status = "Pending" },
                _ => throw new ArgumentException($"Unknown request type: {requestType}")
            };
        }
    }
}


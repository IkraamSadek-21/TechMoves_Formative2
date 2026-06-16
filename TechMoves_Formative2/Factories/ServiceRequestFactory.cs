using TechMoves_Formative2.Models;

namespace TechMoves_Formative2.Factories
{
    public class ServiceRequestFactory : IServiceRequestFactory
    {
        public ServiceRequest CreateRequest(string requestType)
        {
            return requestType switch
            {
                "Transport" => new ServiceRequest
                {
                    RequestType = "Transport",
                    Status = "Pending",
                    Description = "Transport service request"
                },
                "Delivery" => new ServiceRequest
                {
                    RequestType = "Delivery",
                    Status = "Pending",
                    Description = "Delivery service request"
                },
                _ => throw new ArgumentException("Invalid service request type.")
            };
        }
    }
}
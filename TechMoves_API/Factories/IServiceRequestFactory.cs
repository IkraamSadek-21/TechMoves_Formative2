using TechMoves_API.Models;
namespace TechMoves_API.Factories
{
    public interface IServiceRequestFactory
    {
        ServiceRequest CreateRequest(string requestType);
    }
}

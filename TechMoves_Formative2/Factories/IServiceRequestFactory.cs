using TechMoves_Formative2.Models;

namespace TechMoves_Formative2.Factories
{
    public interface IServiceRequestFactory
    {
        ServiceRequest CreateRequest(string requestType);
    }
}
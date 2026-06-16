namespace TechMoves_API.Observers
{
    public interface IContractObserver
    {
        void Update(string contractName, string newStatus);
    }
}

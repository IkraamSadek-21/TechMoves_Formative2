namespace TechMoves_API.Observers
{
    public class ContractStatusObserver : IContractObserver
    {
        public void Update(string contractName, string newStatus)
        {
            Console.WriteLine($"[Observer] Contract '{contractName}' status changed to '{newStatus}'");
        }
    }
}

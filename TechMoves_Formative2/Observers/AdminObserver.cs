using TechMoves_Formative2.Models;

namespace TechMoves_Formative2.Observers
{
    public class AdminObserver : IContractObserver
    {
        public void Update(Contract contract)
        {
            Console.WriteLine($"Admin notified: Contract {contract.ContractID} status changed to {contract.Status}");
        }
    }
}
using TechMoves_Formative2.Models;

namespace TechMoves_Formative2.Observers
{
    public class FinanceObserver : IContractObserver
    {
        public void Update(Contract contract)
        {
            Console.WriteLine($"Finance notified: Contract {contract.ContractID} status changed to {contract.Status}");
        }
    }
}
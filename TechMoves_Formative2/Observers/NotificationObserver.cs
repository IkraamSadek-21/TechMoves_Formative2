using TechMoves_Formative2.Models;

namespace TechMoves_Formative2.Observers
{
    public class NotificationObserver : IContractObserver
    {
        public void Update(Contract contract)
        {
            Console.WriteLine($"Notification sent: Contract {contract.ContractID} changed to {contract.Status}");
        }
    }
}
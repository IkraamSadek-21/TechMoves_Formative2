namespace TechMoves_API.Observers
{
    public class ContractNotifier
    {
        private readonly List<IContractObserver> _observers = new();

        public void Subscribe(IContractObserver observer) => _observers.Add(observer);

        public void Unsubscribe(IContractObserver observer) => _observers.Remove(observer);

        public void Notify(string contractName, string newStatus)
        {
            foreach (var observer in _observers)
                observer.Update(contractName, newStatus);
        }
    }
}

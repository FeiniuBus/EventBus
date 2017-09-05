namespace EventBus.Core
{
    public interface IMessagePersistenterFactory
    {
        long TransactionID { get; }
        IMessagePersistenter<TContent> Create<TContent>();
        IMessagePersistenterFactory CreateScope(bool createServiceProviderScope = false);
    }
}

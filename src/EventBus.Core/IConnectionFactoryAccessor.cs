using RabbitMQ.Client;

namespace EventBus.Core
{
    public interface IConnectionFactoryAccessor
    {
        IConnectionFactory ConnectionFactory { get; }
    }
}

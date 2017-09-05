using EventBus.Core;
using RabbitMQ.Client;
using System.Threading.Tasks;

namespace EventBus.Publish
{
    public class MessageQueueTransaction : IMessageQueueTransaction
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IConnectionFactoryAccessor _connectionFactoryAccessor;

        public MessageQueueTransaction(IConnectionFactoryAccessor connectionFactoryAccessor)
        {
            _connectionFactoryAccessor = connectionFactoryAccessor;
            _connection = _connectionFactoryAccessor.ConnectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.TxSelect();
        }

        public Task CommitAsync()
        {
            _channel.TxCommit();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel.Dispose();
        }

        public Task PublishAsync(string exchange, string routingKey, byte[] body)
        {
            _channel.BasicPublish(exchange, routingKey, null, body);
            return Task.CompletedTask;
        }

        public Task RollbackAsync()
        {
            _channel.TxRollback();
            return Task.CompletedTask;
        }
    }
}

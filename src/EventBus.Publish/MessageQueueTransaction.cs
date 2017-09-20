using EventBus.Core;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventBus.Publish
{
    public class MessageQueueTransaction : IMessageQueueTransaction
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IConnectionFactoryAccessor _connectionFactoryAccessor;
        private readonly IServiceProvider _serviceProvider;

        public MessageQueueTransaction(IConnectionFactoryAccessor connectionFactoryAccessor
            , IServiceProvider serviceProvider)
        {
            _connectionFactoryAccessor = connectionFactoryAccessor;
            _connection = _connectionFactoryAccessor.ConnectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.TxSelect();
            _serviceProvider = serviceProvider;
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

        public async Task PublishAsync(string exchange, string routingKey, byte[] body)
        {
            try
            {
                _channel.ExchangeDeclare(exchange, "topic", true, false, null);
                _channel.BasicPublish(exchange, routingKey, null, body);
            }
            catch
            {
                try
                {
                   await HandleFailureAsync(exchange, routingKey, body);
                }
                catch
                {

                }
            }
        }

        public Task RollbackAsync()
        {
            _channel.TxRollback();
            return Task.CompletedTask;
        }

        private async Task HandleFailureAsync(string exchange, string topic, byte[] content)
        {
            var handlers = _serviceProvider.GetServices<IPubFailureHandler>();
            if (!handlers.Any())
            {
                return;
            }

            foreach(var handler in handlers)
            {
                await handler.HandleAsync(exchange, topic, content);
            }
        }
    }
}

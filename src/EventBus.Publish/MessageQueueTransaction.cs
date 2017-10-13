using EventBus.Core;
using EventBus.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventBus.Publish
{
    public class MessageQueueTransaction : IMessageQueueTransaction
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IConnectionFactoryAccessor _connectionFactoryAccessor;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEnviromentNameConcator _exchangeNameUpdator;
        private readonly ILogger _logger;

        public MessageQueueTransaction(IConnectionFactoryAccessor connectionFactoryAccessor
            , ILogger logger
            , IServiceProvider serviceProvider)
        {
            _connectionFactoryAccessor = connectionFactoryAccessor;
            _serviceProvider = serviceProvider;
            _logger = logger;

            _exchangeNameUpdator = _serviceProvider.GetRequiredService<IEnviromentNameConcator>();
        }

        public Task CommitAsync()
        {
            _channel.TxCommit();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel.Close();
            _channel.Dispose();
            _connection.Close();
            _connection.Dispose();
        }

        public async Task PublishAsync(string exchange, string routingKey, byte[] body)
        {
            OpenConnection();
            exchange = _exchangeNameUpdator.Concat(exchange);
            try
            {
                var property = _channel.CreateBasicProperties();
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
            OpenConnection();
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

        private void OpenConnection()
        {
            if (_connection != null && _connection.IsOpen) return;

            _connection = _connectionFactoryAccessor.ConnectionFactory.CreateConnection();
            _connection.CallbackException += (sender, e) =>
            {
                _logger.LogError(new { Source = nameof(MessageQueueTransaction), Target = "RabbitMQ Connection", Event = "CallbackException",  Errors = e.Exception.GetMessages(), Detail = e.Detail }.ToJson());
            };
            _connection.ConnectionBlocked += (sender, e) =>
            {
                _logger.LogError(new { Source = nameof(MessageQueueTransaction), Target = "RabbitMQ Connection", Event = "ConnectionBlocked", Reason = e.Reason }.ToJson());
            };
            _connection.ConnectionRecoveryError += (sender, e) =>
            {
                _logger.LogError(new { Source = nameof(MessageQueueTransaction), Target = "RabbitMQ Connection", Event = "ConnectionRecoveryError", Errors = e.Exception.GetMessages() }.ToJson());
            };
            _connection.ConnectionShutdown += (sender, e) =>
            {
                _logger.LogError(new { Source = nameof(MessageQueueTransaction), Target = "RabbitMQ Connection", Event = "ConnectionShutdown", Cause = e.Cause, ClassId = e.ClassId, ShutdownInitiator = e.Initiator, e.MethodId, e.ReplyCode, e.ReplyText }.ToJson());
            };
            _connection.ConnectionUnblocked += (sender, e) =>
            {
                _logger.LogInformation(new { Source = nameof(MessageQueueTransaction), Target = "RabbitMQ Connection", Event = "ConnectionUnblocked" }.ToJson());
            };

            _connection.RecoverySucceeded += (sender, e) =>
            {
                _logger.LogInformation(new { Source = nameof(MessageQueueTransaction), Target = "RabbitMQ Connection", Event = "RecoverySucceeded" }.ToJson());
            };

            _channel = _connection.CreateModel();
            _channel.TxSelect();
        }
    }
}

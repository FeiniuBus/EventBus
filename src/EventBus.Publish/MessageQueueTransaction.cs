using EventBus.Core;
using EventBus.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            , ILogger<MessageQueueTransaction> logger
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
            try
            {
                if (_channel != null)
                {
                    _channel.Close();
                    _channel.Dispose();
                }
                if (_connection != null)
                {
                    _connection.Close();
                    _connection.Dispose();
                }
            }
            catch
            {

            }
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
                _logger.LogInformation(new { Target = "Publish to RabbitMQ", Topic = routingKey, Exchange = exchange, ContentLength = body.Length }.ToJson());
            }
            catch(Exception e)
            {
                _logger.LogError(new { Target = "Publish to RabbitMQ", Errors = e.GetMessages() }.ToJson());
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
                _logger.LogInformation(new { Source = nameof(MessageQueueTransaction), Target = "RabbitMQ Connection", Event = "CallbackException",  Errors = e.Exception.GetMessages(), Detail = e.Detail }.ToJson());
            };
            _connection.ConnectionBlocked += (sender, e) =>
            {
                _logger.LogInformation(new { Source = nameof(MessageQueueTransaction), Target = "RabbitMQ Connection", Event = "ConnectionBlocked", Reason = e.Reason }.ToJson());
            };
            _connection.ConnectionRecoveryError += (sender, e) =>
            {
                _logger.LogInformation(new { Source = nameof(MessageQueueTransaction), Target = "RabbitMQ Connection", Event = "ConnectionRecoveryError", Errors = e.Exception.GetMessages() }.ToJson());
            };
            _connection.ConnectionShutdown += (sender, e) =>
            {
                _logger.LogInformation(new { Source = nameof(MessageQueueTransaction), Target = "RabbitMQ Connection", Event = "ConnectionShutdown", Cause = e.Cause, ClassId = e.ClassId, ShutdownInitiator = e.Initiator, e.MethodId, e.ReplyCode, e.ReplyText }.ToJson());
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

            _channel.BasicAcks += (sender, e) => _logger.LogInformation(new { Source = nameof(MessageQueueTransaction), Target = "RabbitMQ Channel", Event = "BasicAcks", e.DeliveryTag, e.Multiple }.ToJson());
            _channel.BasicNacks += (sender, e) => _logger.LogInformation(new { Source = nameof(MessageQueueTransaction), Target = "RabbitMQ Channel", Event = "BasicNacks", e.DeliveryTag, e.Multiple,e.Requeue }.ToJson());
            _channel.BasicRecoverOk += (sender, e) => _logger.LogInformation(new { Source = nameof(MessageQueueTransaction), Target = "RabbitMQ Channel", Event = "BasicRecoverOk"}.ToJson());
            _channel.BasicReturn += (sender, e) => _logger.LogInformation(new { Source = nameof(MessageQueueTransaction), Target = "RabbitMQ Channel", Event = "BasicReturn", ContentLength = e.Body?.Length, e.Exchange, e.ReplyCode,e.ReplyText,e.RoutingKey}.ToJson());
            _channel.CallbackException +=(sender, e) => _logger.LogInformation(new { Source = nameof(MessageQueueTransaction), Target = "RabbitMQ Channel", Event = "CallbackException", e.Detail, Errors = e.Exception.GetMessages() }.ToJson());
            _channel.FlowControl += (sender, e) => _logger.LogInformation(new { Source = nameof(MessageQueueTransaction), Target = "RabbitMQ Channel", Event = "FlowControl", e.Active }.ToJson());
            
            _channel.TxSelect();
        }
    }
}

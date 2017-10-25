using EventBus.Core;
using EventBus.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using EventBus.Core.Extensions;

namespace EventBus.Subscribe.Infrastructure
{
    public class DefaultSubscribeClient : ISubscribeClient
    {
        private IConnection Connection;
        private IModel Channel;
        private readonly IConnectionFactoryAccessor _connectionFactoryAccessor;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DefaultSubscribeClient> _logger;
        private readonly RabbitOptions _rabbitOptions;
        private readonly string _group;
        private readonly string _exchange;

        public Action<MessageContext> OnReceive { get; set; }

        public DefaultSubscribeClient( IServiceProvider serviceProvider
            , IConnectionFactoryAccessor connectionFactoryAccessor
            , RabbitOptions rabbitOptions
            , string group
            , string exchange)
        {
            _serviceProvider = serviceProvider;
            _connectionFactoryAccessor = connectionFactoryAccessor;
            _rabbitOptions = rabbitOptions;
            _group = group;
            _exchange = exchange;

            _logger = _serviceProvider.GetService<ILogger<DefaultSubscribeClient>>();
        }

        private void EnsureChannel()
        {
            if (Channel != null) return;
            EnsureConnection();
            Channel = Connection.CreateModel();

            Channel.BasicAcks += (sender, e) => _logger.LogInformation(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Channel", Event = "BasicAcks", e.DeliveryTag, e.Multiple }.ToJson());
            Channel.BasicNacks += (sender, e) => _logger.LogInformation(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Channel", Event = "BasicNacks", e.DeliveryTag, e.Multiple, e.Requeue }.ToJson());
            Channel.BasicRecoverOk += (sender, e) => _logger.LogInformation(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Channel", Event = "BasicRecoverOk" }.ToJson());
            Channel.BasicReturn += (sender, e) => _logger.LogInformation(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Channel", Event = "BasicReturn", ContentLength = e.Body?.Length, e.Exchange, e.ReplyCode, e.ReplyText, e.RoutingKey }.ToJson());
            Channel.CallbackException += (sender, e) => _logger.LogInformation(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Channel", Event = "CallbackException", e.Detail, Errors = e.Exception.GetMessages() }.ToJson());
            Channel.FlowControl += (sender, e) => _logger.LogInformation(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Channel", Event = "FlowControl", e.Active }.ToJson());

            Channel.ExchangeDeclare(_exchange, "topic", true);
            Channel.ExchangeDeclare(_rabbitOptions.DefaultDeadLetterExchange, "topic", true);
            
            var args = new Dictionary<string, object>
            {
                ["x-message-ttl"] = _rabbitOptions.QueueMessageExpires,
                ["x-dead-letter-exchange"] = _rabbitOptions.DefaultDeadLetterExchange,
            };

            Channel.QueueDeclare(
                _group
                , true
                , false
                , false
                , args);
        }

        private void EnsureConnection()
        {
            if (Connection != null) return;
            
            var factory = _connectionFactoryAccessor.ConnectionFactory;

            Connection = factory.CreateConnection();

            Connection.CallbackException += (sender, e) =>
            {
                _logger.LogError(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Connection", Event = "CallbackException", Errors = e.Exception.GetMessages(), Detail = e.Detail }.ToJson());
            };
            Connection.ConnectionBlocked += (sender, e) =>
            {
                _logger.LogError(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Connection", Event = "ConnectionBlocked", Reason = e.Reason }.ToJson());
            };
            Connection.ConnectionRecoveryError += (sender, e) =>
            {
                _logger.LogError(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Connection", Event = "ConnectionRecoveryError", Errors = e.Exception.GetMessages() }.ToJson());
            };
            Connection.ConnectionShutdown += (sender, e) =>
            {
                _logger.LogError(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Connection", Event = "ConnectionShutdown", Cause = e.Cause, ClassId = e.ClassId, ShutdownInitiator = e.Initiator, e.MethodId, e.ReplyCode, e.ReplyText }.ToJson());
            };
            Connection.ConnectionUnblocked += (sender, e) =>
            {
                _logger.LogInformation(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Connection", Event = "ConnectionUnblocked" }.ToJson());
            };

            Connection.RecoverySucceeded += (sender, e) =>
            {
                _logger.LogInformation(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Connection", Event = "RecoverySucceeded" }.ToJson());
            };
        }

        public void Subscribe(string[] topics)
        {
            if (topics == null) throw new ArgumentNullException(nameof(topics));

            _logger.LogInformation($"subscribe topics {topics.ToJson()}");

            EnsureChannel();

            foreach (var topic in topics)
            {
                Channel.QueueBind(_group, _exchange, topic);
            }
        }

        public void Listening()
        {
            EnsureChannel();
            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += OnConsumerReceived;
            consumer.ConsumerCancelled += (sender, e) => _logger.LogInformation(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Consumer", Event = "ConsumerCancelled", e.ConsumerTag }.ToJson());
            consumer.Received += (sender, e) => _logger.LogInformation(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Consumer", Event = "Received", e.ConsumerTag, e.DeliveryTag, e.Exchange,e.Redelivered,e.RoutingKey, ContentLength = e.Body.Length }.ToJson());
            consumer.Registered += (sender, e) => _logger.LogInformation(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Consumer", Event = "Registered", e.ConsumerTag }.ToJson());
            consumer.Shutdown +=(sender, e) => _logger.LogInformation(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Consumer", Event = "Shutdown", e.Cause, e.ClassId, e.MethodId, e.ReplyCode, e.ReplyText }.ToJson());
            consumer.Unregistered += (sender, e) => _logger.LogInformation(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Consumer", Event = "Unregistered", e.ConsumerTag }.ToJson());
            Channel.BasicConsume(_group, false, consumer);
        }

        public void Dispose()
        {
            Channel?.Dispose();
            Connection?.Dispose();
            Connection?.Close();
        }

        public void OnConsumerReceived(object sender, BasicDeliverEventArgs e)
        {
            var context = new MessageContext
            {
                Exchange = _exchange,
                Topic = e.RoutingKey,
                Queue = _group,
                DeliveryTag = e.DeliveryTag,
                Channel = Channel,
                Content = e.Body
            };

            _logger.LogInformation($"receive messages {context.ToJson()}");

            OnReceive?.Invoke(context);
        }

        private void OpenConnection()
        {
            if (Connection != null && Connection.IsOpen) return;

            Connection = _connectionFactoryAccessor.ConnectionFactory.CreateConnection();
            Connection.CallbackException += (sender, e) =>
            {
                _logger.LogError(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Connection", Event = "CallbackException", Errors = e.Exception.GetMessages(), Detail = e.Detail }.ToJson());
            };
            Connection.ConnectionBlocked += (sender, e) =>
            {
                _logger.LogError(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Connection", Event = "ConnectionBlocked", Reason = e.Reason }.ToJson());
            };
            Connection.ConnectionRecoveryError += (sender, e) =>
            {
                _logger.LogError(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Connection", Event = "ConnectionRecoveryError", Errors = e.Exception.GetMessages() }.ToJson());
            };
            Connection.ConnectionShutdown += (sender, e) =>
            {
                _logger.LogError(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Connection", Event = "ConnectionShutdown", Cause = e.Cause, ClassId = e.ClassId, ShutdownInitiator = e.Initiator, e.MethodId, e.ReplyCode, e.ReplyText }.ToJson());
            };
            Connection.ConnectionUnblocked += (sender, e) =>
            {
                _logger.LogInformation(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Connection", Event = "ConnectionUnblocked" }.ToJson());
            };
            Connection.RecoverySucceeded += (sender, e) =>
            {
                _logger.LogInformation(new { Source = nameof(DefaultSubscribeClient), Target = "RabbitMQ Connection", Event = "RecoverySucceeded" }.ToJson());
            };
        }
    }
}

using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using EventBus.Core.Extensions;

namespace EventBus.Core.Infrastructure
{
    public class FailureClient : IFailureClient
    {
        private IConnection Connection;
        private IModel Channel;
        private readonly IConnectionFactoryAccessor _connectionFactoryAccessor;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FailureClient> _logger;
        private readonly RabbitOptions _rabbitOptions;
        private readonly string _group;
        private readonly string _exchange;

        public Action<MessageContext> OnReceive { get; set; }

        public FailureClient( IServiceProvider serviceProvider
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

            _logger = _serviceProvider.GetService<ILogger<FailureClient>>();
        }

        private void EnsureChannel()
        {
            if (Channel != null) return;
            EnsureConnection();
            Channel = Connection.CreateModel();

            Channel.ExchangeDeclare(_exchange, "topic", true);
            Channel.ExchangeDeclare(_rabbitOptions.DefaultFinalDeadLetterExchange, "topic", true, false, new Dictionary<string, object>
            {
                ["x-message-ttl"] = _rabbitOptions.QueueMessageExpires,
            });

            var args = new Dictionary<string, object>
            {
                ["x-message-ttl"] = _rabbitOptions.QueueMessageExpires,
                ["x-dead-letter-exchange"] = _rabbitOptions.DefaultFinalDeadLetterExchange,
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
        }

        public void Subscribe(string[] topics)
        {
            if (topics == null) throw new ArgumentNullException(nameof(topics));

            _logger.LogInformation($"failure callback topics {topics.ToJson()}");

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
                Content = e.Body,
                Args = e
            };

            _logger.LogInformation($"failure context: {context.ToJson()}");

            OnReceive?.Invoke(context);
        }
    }
}

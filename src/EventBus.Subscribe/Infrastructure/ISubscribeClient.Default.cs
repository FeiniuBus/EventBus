using EventBus.Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace EventBus.Subscribe.Infrastructure
{
    public class DefaultSubscribeClient : ISubscribeClient
    {
        private IConnection Connection;
        private IModel Channel;
        private readonly IConnectionFactoryAccessor _connectionFactoryAccessor;
        private readonly RabbitOptions _rabbitOptions;
        private readonly string _group;
        private readonly string _exchange;

        public Action<SubscribeContext> OnReceive { get; set; }

        public DefaultSubscribeClient(IConnectionFactoryAccessor connectionFactoryAccessor
            , RabbitOptions rabbitOptions
            , string group
            , string exchange)
        {
            _connectionFactoryAccessor = connectionFactoryAccessor;
            _rabbitOptions = rabbitOptions;
            _group = group;
            _exchange = exchange;
        }

        private void EnsureChannel()
        {
            if (Channel != null) return;
            EnsureConnection();
            Channel = Connection.CreateModel();

            Channel.ExchangeDeclare(_exchange
                , "topic"
                , true);

            var args = new Dictionary<string, object>
            {
                ["x-message-ttl"] = _rabbitOptions.QueueMessageExpires
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
            var context = new SubscribeContext
            {
                Exchange = _exchange,
                Topic = e.RoutingKey,
                Queue = _group,
                DeliveryTag = e.DeliveryTag,
                Channel = Channel,
                Content = e.Body
            };
            OnReceive?.Invoke(context);
        }
    }
}

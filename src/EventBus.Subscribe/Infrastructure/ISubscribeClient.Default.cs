﻿using EventBus.Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBus.Subscribe.Infrastructure
{
    public class DefaultSubscribeClient : ISubscribeClient
    {
        private readonly IList<IDisposable> _disposables;
        private readonly IList<IModel> _channels;
        private readonly IConnectionFactoryAccessor _connectionFactoryAccessor;
        private readonly SubscribeInfoCache _cache;

        public Action<SubscribeContext> OnReceive { get; set; }

        public DefaultSubscribeClient(IConnectionFactoryAccessor connectionFactoryAccessor
            , SubscribeInfoCache cache)
        {
            _disposables = new List<IDisposable>();
            _channels = new List<IModel>();
            _connectionFactoryAccessor = connectionFactoryAccessor;
            _cache = cache;
        }

        public void Start()
        {
            EnsureChannel();
        }

        private void EnsureChannel()
        {
            if (_channels.Any()) return;

            var factory = _connectionFactoryAccessor.ConnectionFactory;
            var connection = factory.CreateConnection();
            connection.AutoClose = true;

            foreach(var nameGroupe in _cache.Entries)
            {   
                foreach(var groupGroup in nameGroupe.Value)
                {
                    var channel = connection.CreateModel();
                    _channels.Add(channel);
                    _disposables.Add(channel);

                    channel.ExchangeDeclare(
                        nameGroupe.Key
                        , "topic"
                        , true
                        , false
                        , null);

                    channel.QueueDeclare(
                        groupGroup.Key
                        , true
                        , false
                        , false
                        , null);

                    channel.QueueBind(groupGroup.Key, nameGroupe.Key, groupGroup.Value.Key, null);

                    EnsureConsumer(channel, groupGroup.Key);
                }
            }
        }

        private void EnsureConsumer(IModel channel, string queue)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var context = new SubscribeContext
                {
                    Name = e.Exchange,
                    Key = e.RoutingKey,
                    Queue = queue,
                    DeliveryTag = e.DeliveryTag,
                    Channel = channel
                };
                
                OnReceive?.Invoke(context);
            };

            channel.BasicConsume(queue, false, consumer);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }

        public void Ack(SubscribeContext context)
        {
            context.Channel.BasicAck(context.DeliveryTag, false);
        }

        public void Reject(SubscribeContext context)
        {
            context.Channel.BasicReject(context.DeliveryTag, true);
        }
    }
}

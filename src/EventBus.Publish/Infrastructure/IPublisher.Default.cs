using EventBus.Core;
using EventBus.Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventBus.Publish.Infrastructure
{
    public class DefaultPublisher : IPublisher
    {
        private readonly MessageInfoCache _messageInfoCache;
        private readonly IMessageSerializer _messageSerializer;
        private readonly IConnectionFactoryAccessor _connectionFactoryAccessor;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitOptions _rabbitOptions;
        private IModel Channel;

        public DefaultPublisher(
            MessageInfoCache messageInfoCache
            , IMessageSerializer messageSerializer
            , IConnectionFactoryAccessor connectionFactoryAccessor
            , IServiceProvider serviceProvider
            , IOptions<RabbitOptions> rabbitOptions)
        {
            _messageInfoCache = messageInfoCache;
            _messageSerializer = messageSerializer;
            _connectionFactoryAccessor = connectionFactoryAccessor;
            _serviceProvider = serviceProvider;
            _rabbitOptions = rabbitOptions.Value;
        }

        public Task PublishAsync<MessageT>(MessageT message) where MessageT: class
        {
            EnsureMessage(message);
            EnsureChannel();

            Channel.BasicPublish(
                _messageInfoCache.GetRequiredMessageName<MessageT>()
                , _messageInfoCache.GetMessageKey<MessageT>()
                , null
                , _messageSerializer.Serialize(message)
                );

            return Task.CompletedTask;
        }

        public Task PublishAsync<MessageT>(MessageT message, IDictionary<string, object> args) where MessageT: class
        {
            EnsureMessage(message);
            EnsureChannel();

            Channel.BasicPublish(
                _messageInfoCache.GetMessageName<MessageT>() ?? _rabbitOptions.DefaultExchangeName
                , _messageInfoCache.GetMessageKey<MessageT>()
                , null
                , _messageSerializer.Serialize(message)
                );

            return Task.CompletedTask;
        }

        private void EnsureChannel()
        {
            if (Channel != null) return;
            InitChannal();
        }

        private void EnsureMessage<MessageT>(MessageT mesage) where MessageT: class
        {
            var validators = _serviceProvider.GetServices<IPubMessageValidator>();
            var context = new PubMessageValidateContext
            {
                MessageType = typeof(MessageT),
                Message = mesage,
            };

            foreach(var validator in validators)
            {
                validator.Validate(context);

                if (context.Result.Failed)
                {
                    throw new InvalidOperationException(context.Result.Errors.Last());
                }
            }
        }

        private void InitChannal()
        {
            var factory = _connectionFactoryAccessor.ConnectionFactory;
            var connection = factory.CreateConnection();
            connection.AutoClose = true;
            Channel = connection.CreateModel();
        }

        public void Dispose()
        {
            Channel?.Dispose();
        }
    }
}

using EventBus.Core;
using EventBus.Core.Infrastructure;
using EventBus.Subscribe.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBus.Subscribe.Infrastructure
{
    public class DefaultSubscribeConsumer : IConsumer
    {
        private readonly IList<IDisposable> _disposables;
        private readonly IServiceProvider _serviceProvider;
        private readonly SubscribeOptions _subscribeOptions;
        private readonly IReceivedEventPersistenter _receivedEventPersistenter;

        public DefaultSubscribeConsumer(IServiceProvider serviceProvider
            , IReceivedEventPersistenter receivedEventPersistenter
            , IOptions<SubscribeOptions> subscribeOptionsAccessor)
        {
            _disposables = new List<IDisposable>();
            _serviceProvider = serviceProvider;
            _subscribeOptions = subscribeOptionsAccessor.Value;
            _receivedEventPersistenter = receivedEventPersistenter;
        }

        public void Start()
        {
            var clients = GetClients();
            foreach(var client in clients)
            {
                client.Listening();
            }
        }

        private ISubscribeClient[] GetClients()
        {
            var subscribeInfos = _subscribeOptions.SubscribeInfos;
            var groupedInfos = subscribeInfos.GroupBy(x => x.Group)
                .ToDictionary(x => x.Key, x => x.GroupBy(y => y.Exchange)
                    .ToDictionary(y => y.Key, y => y.ToArray()));

            var clients = new List<ISubscribeClient>();
            var connectfactoryAccessor = _serviceProvider.GetRequiredService<IConnectionFactoryAccessor>();
            var rabbitOption = _serviceProvider.GetRequiredService<IOptions<RabbitOptions>>().Value;

            foreach(var queueGroupedItems in groupedInfos)
            {
                foreach(var exchangeGroupedItems in queueGroupedItems.Value)
                {
                    var exchange = string.IsNullOrEmpty(exchangeGroupedItems.Key) ? rabbitOption.DefaultExchangeName : exchangeGroupedItems.Key;

                    var client = new DefaultSubscribeClient(connectfactoryAccessor
                        , rabbitOption
                        , queueGroupedItems.Key
                        , exchange);

                    _disposables.Add(client);
                    RegisterClient(client);

                    var topics = exchangeGroupedItems.Value.Select(x => x.Topic).ToArray();
                    client.Subscribe(topics);

                    client.Listening();
                }
            }

            return clients.ToArray();
        }

        private void RegisterClient(ISubscribeClient client)
        {
            client.OnReceive = (MessageContext context) =>
            {
                try
                {
                    _receivedEventPersistenter.InsertAsync(context.Content);
                }
                catch
                {
                    context.Reject(true);
                    return;
                }

                bool result = false;
                try
                {
                    var invoker = new DefaultConsumerInvoker(_serviceProvider, context);
                    result = invoker.InvokeAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                }
                finally
                {
                    if (result)
                    {
                        context.Ack();
                    }
                    else
                    {
                        context.Reject();
                    }
                }
            };
        }

        public void Dispose()
        {
            foreach(var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}

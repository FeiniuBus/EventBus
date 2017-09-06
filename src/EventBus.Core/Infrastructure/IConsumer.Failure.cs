using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using EventBus.Core.Internal;

namespace EventBus.Core.Infrastructure
{
    public class FailureConsumer : IConsumer
    {
        private readonly IList<IDisposable> _disposables;
        private readonly IServiceProvider _serviceProvider;
        private readonly FailureHandleOptions _subscribeOptions;

        public FailureConsumer(IServiceProvider serviceProvider
            , FailureHandleOptions subscribeOptionsAccessor)
        {
            _disposables = new List<IDisposable>();
            _serviceProvider = serviceProvider;
            _subscribeOptions = subscribeOptionsAccessor;
        }

        public void Start()
        {
            var clients = GetClients();
            foreach (var client in clients)
            {
                client.Listening();
            }
        }

        private IClient[] GetClients()
        {
            var subscribeInfos = _subscribeOptions.DeadLetterInfos;
            var groupedInfos = subscribeInfos.GroupBy(x => x.Group)
                .ToDictionary(x => x.Key, x => x.GroupBy(y => y.Exchange)
                    .ToDictionary(y => y.Key, y => y.ToArray()));

            var clients = new List<IClient>();
            var connectfactoryAccessor = _serviceProvider.GetRequiredService<IConnectionFactoryAccessor>();
            var rabbitOption = _serviceProvider.GetRequiredService<IOptions<RabbitOptions>>().Value;

            foreach (var queueGroupedItems in groupedInfos)
            {
                foreach (var exchangeGroupedItems in queueGroupedItems.Value)
                {
                    var exchange = string.IsNullOrEmpty(exchangeGroupedItems.Key) ? rabbitOption.DefaultExchangeName : exchangeGroupedItems.Key;

                    var client = new FailureClient(connectfactoryAccessor
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

        private void RegisterClient(IClient client)
        {
            client.OnReceive = (MessageContext context) =>
            {
                bool result = false;
                try
                {
                    var invoker = new FailureInvoker(_serviceProvider, context);
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
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}

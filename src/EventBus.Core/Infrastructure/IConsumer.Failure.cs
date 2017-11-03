using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using EventBus.Core.Internal;
using Microsoft.Extensions.Logging;
using EventBus.Core.Extensions;

namespace EventBus.Core.Infrastructure
{
    public class FailureConsumer : IConsumer
    {
        private readonly IList<IDisposable> _disposables;
        private readonly IServiceProvider _serviceProvider;
        private readonly FailureHandleOptions _subscribeOptions;
        private readonly ILogger<FailureConsumer> _logger;

        public FailureConsumer(IServiceProvider serviceProvider
            , ILogger<FailureConsumer> logger)
        {
            _disposables = new List<IDisposable>();
            _serviceProvider = serviceProvider;
            _subscribeOptions = _serviceProvider.GetService<FailureHandleOptions>();
            _logger = logger;
        }

        public void Start()
        {
            var clients = GetClients();
            foreach (var client in clients)
            {
                client.Listening();
            }
        }

        private IList<IClient> GetClients()
        {
            if (_subscribeOptions == null) return new IClient[] { };

            var subscribeInfos = _subscribeOptions.DeadLetterInfos;
            var groupedInfos = subscribeInfos.GroupBy(x => x.Group)
                .ToDictionary(x => x.Key, x => x.GroupBy(y => y.Exchange)
                    .ToDictionary(y => y.Key, y => y.ToArray()));

            _logger.LogInformation($"deadletter subscribeinfos {subscribeInfos.ToJson()}");

            var clients = new List<IClient>();
            var connectfactoryAccessor = _serviceProvider.GetRequiredService<IConnectionFactoryAccessor>();
            var rabbitOption = _serviceProvider.GetRequiredService<IOptions<RabbitOptions>>().Value;

            foreach (var queueGroupedItems in groupedInfos)
            {
                foreach (var exchangeGroupedItems in queueGroupedItems.Value)
                {
                    var exchange = string.IsNullOrEmpty(exchangeGroupedItems.Key) ? rabbitOption.DefaultExchangeName : exchangeGroupedItems.Key;

                    var client = new FailureClient( _serviceProvider
                        , connectfactoryAccessor
                        , rabbitOption
                        , queueGroupedItems.Key
                        , exchange);

                    _disposables.Add(client);
                    clients.Add(client);
                    RegisterClient(client);

                    var topics = exchangeGroupedItems.Value.Select(x => x.Topic).ToArray();
                    client.Subscribe(topics);
                }
            }

            return clients;
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
                catch (Exception ex)
                {
                    _logger.LogError(110, ex, $"invoke deadletter fail: {context.ToJson()}");
                }
                finally
                {
                    if (result)
                    {
                        context.Ack();
                        _logger.LogInformation($"ack deadletter message {context.ToJson()}");
                    }
                    else
                    {
                        context.Reject();
                        _logger.LogInformation($"reject deadletter message {context.ToJson()}");
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

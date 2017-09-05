using EventBus.Subscribe.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBus.Subscribe.Infrastructure
{
    public class DefaultSubscribeConsumer : ISubscribeConsumer
    {
        private readonly IList<IDisposable> _disposables;
        private readonly IServiceProvider _serviceProvider;

        public DefaultSubscribeConsumer(IServiceProvider serviceProvider)
        {
            _disposables = new List<IDisposable>();
            _serviceProvider = serviceProvider;
        }

        public void Start()
        {
            var clients = GetClients();
            foreach(var client in clients)
            {
                client.Start();
            }
        }

        private ISubscribeClient[] GetClients()
        {
            var clients = _serviceProvider.GetServices<ISubscribeClient>().ToArray();
            foreach(var client in clients)
            {
                _disposables.Add(client);
            }
            return clients;
        }

        private void RegisterClient(ISubscribeClient client)
        {
            client.OnReceive = (SubscribeContext context) =>
            {
                try
                {
                    var invoker = new DefaultConsumerInvoker(_serviceProvider, context);
                    invoker.InvokeAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    client.Ack(context);
                }
                catch
                {
                    client.Reject(context);
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

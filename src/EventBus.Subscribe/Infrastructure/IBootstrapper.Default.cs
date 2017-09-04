using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBus.Subscribe.Infrastructure
{
    public class DefaultBootstrapper : IBootstrapper
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly IList<IDisposable> _disposables; 

        public DefaultBootstrapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _disposables = new List<IDisposable>();
        }

        public void Start()
        {
            StartSubscribeClients();
        }

        private ISubscribeClient[] GetSubscribeClients()
        {
            var clients = _serviceProvider.GetServices<ISubscribeClient>().ToArray();
            foreach(var client in clients)
            {
                _disposables.Add(client);
            }

            return clients;
        }

        private void StartSubscribeClients()
        {
            var subscribeClients = GetSubscribeClients();
            foreach(var client in subscribeClients)
            {
                client.Start();
            }
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

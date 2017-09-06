using EventBus.Core;
using EventBus.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var ensureCreate = EnsureCreateAsync();
            ensureCreate.Synchronize();

            StartConsumer();
        }

        private void StartConsumer()
        {
            var consumers = _serviceProvider.GetServices<IConsumer>();
            foreach(var consumer in consumers)
            {
                _disposables.Add(consumer);
                consumer.Start();
            }
        }

        private async Task EnsureCreateAsync()
        {
            var published = _serviceProvider.GetRequiredService<IPublishedEventPersistenter>();
            var received = _serviceProvider.GetRequiredService<IReceivedEventPersistenter>();
            await published.EnsureCreatedAsync();
            await received.EnsureCreatedAsync();
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

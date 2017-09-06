using EventBus.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

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

        public void Dispose()
        {
            foreach(var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}

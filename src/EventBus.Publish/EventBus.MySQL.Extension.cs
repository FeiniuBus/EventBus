using EventBus.Core;
using EventBus.Core.Infrastructure;
using EventBus.Publish.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventBus.Publish
{
    public class EventBusMySQLExtension : IEventBusOptionsExtension
    {
        private readonly Action<IServiceProvider, EventBusMySQLOptions> _configure;

        public EventBusMySQLExtension(Action<IServiceProvider, EventBusMySQLOptions> configure)
        {
            _configure = configure;
        }

        public void AddServices(IServiceCollection serviceCollection)
        {
            var options = new EventBusMySQLOptions();
            _configure(serviceCollection.BuildServiceProvider(), options);

            serviceCollection.AddSingleton(options);
            serviceCollection.AddScoped<IPublishedEventPersistenter, PublishedEventPersistenter>();
            serviceCollection.AddScoped<IIdentityGenerator, IdentityGenerator>();
            serviceCollection.AddScoped<IConnectionFactoryAccessor, DefaultConnectionFactoryAccessor>();
            serviceCollection.AddScoped<IEventPublisher, EventPublisher>();
        }
    }
}

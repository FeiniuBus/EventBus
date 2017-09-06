using EventBus.Core;
using EventBus.Core.Infrastructure;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtenssion
    {
        public static void AddEventBus(this IServiceCollection serviceCollection, Action<EventBusOptions> configure)
        {
            var options = new EventBusOptions();
            configure(options);

            serviceCollection.AddScoped<IMessageSerializer, DefaultMessageSerializer>();

            foreach(var extension in options.Extensions)
            {
                extension.AddServices(serviceCollection);
            }
        }
    }
}

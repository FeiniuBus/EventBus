using EventBus.Core;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtenssion
    {
        public static void AddEventBus(this IServiceCollection serviceCollection, Action<EventBusOptions> configure)
        {
            var options = new EventBusOptions();
            configure(options);

            foreach(var extension in options.Extensions)
            {
                extension.AddServices(serviceCollection);
            }
        }
    }
}

using EventBus.Core;
using EventBus.Core.Infrastructure;
using EventBus.Subscribe.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            serviceCollection.AddScoped<IConsumer, FailureConsumer>();
            serviceCollection.AddSingleton<IBootstrapper, DefaultBootstrapper>();
            serviceCollection.TryAddTransient<IMessageDecoder, DefaultMessageDecoder>();
            serviceCollection.TryAddScoped<IFailureContextAccessor, DefaultFailureContextAccessor>();
            serviceCollection.AddSingleton<IEnviromentNameConcator, DefaultEnviromentalNameUpdator>();

            foreach (var extension in options.Extensions)
            {
                extension.AddServices(serviceCollection);
            }
        }
    }
}

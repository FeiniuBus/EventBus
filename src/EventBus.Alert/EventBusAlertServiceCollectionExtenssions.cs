using EventBus.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventBus.Alert
{
    public static class EventBusAlertServiceCollectionExtenssions
    {
        public static IServiceCollection AddEventBusAlert(this IServiceCollection services, Action<SMSAlertOptions> setup)
        {
            services.Configure(setup);
            services.AddScoped<ISubFailureHandler, DefaultSubAlertHandler>();
            services.AddScoped<IPubFailureHandler, DefaultPubAlertHandler>();
            services.AddSingleton<ILastAlertMemento, DefaultLastAlertMemento>();

            return services;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventBus.Core
{
    public static class ServiceCollectionExtenssion
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, Action<RabbitOptions> setup)
        {
            services.Configure(setup);
            return services;
        }
    }
}

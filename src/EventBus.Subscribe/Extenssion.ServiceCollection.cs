using EventBus.Subscribe.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace EventBus.Subscribe
{
    public static class ServiceCollectionExtenssion
    {
        public static IServiceCollection AddSub(this IServiceCollection services, Action<SubscribeOptions> setup)
        {
            services.Configure(setup);

            services.AddSingleton<IBootstrapper, DefaultBootstrapper>();
            services.AddScoped<ISubscribeConsumer, DefaultSubscribeConsumer>();
            services.AddTransient<IMessageDeSerializer, DefaultMessageDeSerializer>();
            services.AddSingleton<SubscribeInfoCache>();

            services.ConfiguraClients();

            return services;
        }

        private static IServiceCollection ConfiguraClients(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<SubscribeOptions>>().Value;

            for(var i = 0; i < options.ConsumerClientCount; ++i)
            {
                services.AddScoped<ISubscribeClient, DefaultSubscribeClient>();
            }

            return services;
        }
    }
}

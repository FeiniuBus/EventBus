using EventBus.Core;
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

            //services.AddSingleton<IBootstrapper, DefaultBootstrapper>();
            services.AddScoped<IConsumer, DefaultSubscribeConsumer>();
            services.AddTransient<IMessageDeSerializer, DefaultMessageDeSerializer>();
            services.AddScoped<IReceivedEventPersistenter, ReceivedEventPersistenter>();

            services.ConfigurationSubscribeCallbacks();

            return services;
        }

        public static IServiceCollection ConfigurationSubscribeCallbacks(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<SubscribeOptions>>().Value;
            foreach(var info in options.SubscribeInfos)
            {
                services.AddScoped(info.CallbackType, info.CallbackType);
            }
            return services;
        }
    }
}

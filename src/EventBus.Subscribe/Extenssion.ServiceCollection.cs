using EventBus.Core;
using EventBus.Subscribe.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;

namespace EventBus.Subscribe
{
    public static class ServiceCollectionExtenssion
    {
        public static IServiceCollection AddSub(this IServiceCollection services, Action<SubscribeOptions> setup)
        {
            if (setup == null)
            {
                throw new ArgumentNullException($"setup");
            }
            var options = new SubscribeOptions();
            setup(options);

            services.AddSingleton(options);

            services.AddScoped<IConsumer, DefaultSubscribeConsumer>();
            services.AddTransient<IMessageDeSerializer, DefaultMessageDeSerializer>();
            services.AddScoped<IReceivedEventPersistenter, ReceivedEventPersistenter>();
            services.TryAddTransient<IMessageDecoder, DefaultMessageDecoder>();

            services.ConfigurationSubscribeCallbacks();

            return services;
        }

        public static IServiceCollection ConfigurationSubscribeCallbacks(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<SubscribeOptions>();
            options.Build(provider);
            foreach(var info in options.SubscribeInfos)
            {
                services.AddScoped(info.CallbackType, info.CallbackType);
            }
            return services;
        }
    }
}

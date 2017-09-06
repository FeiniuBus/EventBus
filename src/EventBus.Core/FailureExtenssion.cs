using System;
using Microsoft.Extensions.DependencyInjection;
using EventBus.Core.Infrastructure;

namespace EventBus.Core
{
    public class FailureExtenssion : IEventBusOptionsExtension
    {
        private readonly Action<FailureHandleOptions> _configure;

        public FailureExtenssion(Action<FailureHandleOptions> configure)
        {
            _configure = configure;
        }

        public void AddServices(IServiceCollection serviceCollection)
        {
            var options = new FailureHandleOptions();
            _configure(options);

            var provider = serviceCollection.BuildServiceProvider();
            var rabbitOptions = provider.GetRequiredService<RabbitOptions>();

            options.BuildWithDefaultSelfExchangeName(rabbitOptions.DefaultExchangeName, rabbitOptions.DefaultDeadLetterExchange);

            foreach(var item in options.DeadLetterInfos)
            {
                serviceCollection.AddScoped(item.CallbackType, item.CallbackType);
            }

            serviceCollection.AddSingleton(options);
        }
    }
}

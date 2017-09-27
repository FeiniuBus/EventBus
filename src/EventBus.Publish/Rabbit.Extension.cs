using EventBus.Core;
using EventBus.Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventBus.Publish
{
    public class RabbitExtension : IEventBusOptionsExtension
    {
        private readonly Action<RabbitOptions> _configure;

        public RabbitExtension(Action<RabbitOptions> configure)
        {
            _configure = configure;
        }

        public void AddServices(IServiceCollection serviceCollection)
        {
            var options = new RabbitOptions();
            _configure(options);

            var envConcator = serviceCollection.BuildServiceProvider().GetRequiredService<IEnviromentNameConcator>();
            options.DefaultDeadLetterExchange = envConcator.Concat(options.DefaultDeadLetterExchange);
            options.DefaultFinalDeadLetterExchange = envConcator.Concat(options.DefaultFinalDeadLetterExchange);
            options.DefaultExchangeName = envConcator.Concat(options.DefaultExchangeName);

            serviceCollection.AddSingleton(options);
            serviceCollection.AddScoped<IMessageQueueTransaction, MessageQueueTransaction>();
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using EventBus.Core;
using EventBus.Core.Infrastructure;
using System;
using EventBus.Publish.Infrastructure;

namespace EventBus.Publish
{
    public static class ServiceCollectionExtenssion
    {
        public static IServiceCollection AddPub(this IServiceCollection services, Action<PublishOptions> setup)
        {
            services.Configure(setup);

            services.AddSingleton<IConnectionFactoryAccessor, DefaultConnectionFactoryAccessor>();

            services.AddTransient<IMessageSerializer, DefaultMessageSerializer>();
            services.AddTransient<IPubMessageValidator, DefaultPubMessageValidator>();

            services.AddSingleton<MessageInfoCache, MessageInfoCache>();

            services.AddScoped<IPublisher, DefaultPublisher>();

            var assemblyVisitor = new AssemblyVisitor();
            assemblyVisitor.Start();
            services.AddSingleton(assemblyVisitor);

            return services;
        }
    }
}

using EventBus.Subscribe.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Reflection;

namespace EventBus.Subscribe.Internal
{
    internal class DefaultConsumerInvoker : IConsumerInvoker
    {
        private readonly IServiceScope _serviceScope;
        private readonly IServiceProvider _serviceProvider;

        public SubscribeContext Context { get; }

        public DefaultConsumerInvoker(IServiceProvider serviceProvider
            , SubscribeContext context)
        {
            _serviceScope = serviceProvider.CreateScope();
            _serviceProvider = _serviceScope.ServiceProvider;
            Context = context;
        }

        public Task InvokeAsync()
        {
            var subscribeInfo = GetSubscribe();
            var handler = GetHandler(subscribeInfo);
            var message = GetMessage(subscribeInfo);
            return Call(handler, message);
        }

        private Task Call(object handler, object message)
        {
            var method = handler.GetType().GetTypeInfo().GetMethod(nameof(ISubscribeHandler<object>.HandleAsync));
            var task = method.Invoke(handler, new[] { message }) as Task;
            return task;
        }

        private object GetHandler(SubscribeInfo subscribeInfo)
        {
            return _serviceProvider.GetService(subscribeInfo.BaseType);
        }

        private SubscribeInfo GetSubscribe()
        {
            var cache = _serviceProvider.GetService<SubscribeInfoCache>();
            var subscribeInfo = cache.GetSubscribeInfo(Context.Name, Context.Queue);
            return subscribeInfo;
        }

        private object GetMessage(SubscribeInfo subscribeInfo)
        {
            var deserilizer = _serviceProvider.GetService<IMessageDeSerializer>();
            var message = deserilizer.Deserialize(Context.Content, GetMessageType(subscribeInfo));
            return message;
        }

        private Type GetMessageType(SubscribeInfo subscribeInfo)
        {
            var messageType = subscribeInfo.HandlerType.GenericTypeArguments[0];
            return messageType;
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}

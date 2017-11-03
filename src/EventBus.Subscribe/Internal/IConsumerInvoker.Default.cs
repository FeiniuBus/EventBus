using EventBus.Subscribe.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Text;
using EventBus.Core.Infrastructure;
using EventBus.Core;
using EventBus.Core.Util;

namespace EventBus.Subscribe.Internal
{
    internal class DefaultConsumerInvoker : IInvoker
    {
        private readonly IServiceScope _serviceScope;
        private readonly IServiceProvider _serviceProvider;
        private readonly SubscribeOptions _subscribeOptions;

        public MessageContext Context { get; }

        public DefaultConsumerInvoker(IServiceProvider serviceProvider
            , MessageContext context)
        {
            _serviceScope = serviceProvider.CreateScope();
            _serviceProvider = _serviceScope.ServiceProvider;
            _subscribeOptions = _serviceProvider.GetRequiredService<SubscribeOptions>();
            Context = context;
        }

        public Task<bool> InvokeAsync()
        {
            var subscribeInfo = GetSubscribe();
            var handler = GetHandler(subscribeInfo);
            var message = GetMessage(subscribeInfo);
            return Call(handler, message);
        }

        private Task<bool> Call(object handler, object message)
        {
            var method = handler.GetType().GetTypeInfo().GetMethod(nameof(ISubscribeHandler.HandleAsync));
            var task = method.Invoke(handler, new[] { message }) as Task<bool>;
            return task;
        }

        private object GetHandler(SubscribeInfo subscribeInfo)
        {
            return _serviceProvider.GetService(subscribeInfo.CallbackType);
        }

        private SubscribeInfo GetSubscribe()
        {
            return _subscribeOptions.SubscribeInfos.FirstOrDefault(x => x.Exchange == Context.Exchange
                && TopicMatch.IsTopicMatch(Context.Topic, x.Topic)
                && x.Group == Context.Queue);
        }

        private string GetMessage(SubscribeInfo subscribeInfo)
        {
            var receivedMsg = _serviceProvider.GetRequiredService<IMessageDecoder>().Decode(Context);
            return receivedMsg.Content;
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}

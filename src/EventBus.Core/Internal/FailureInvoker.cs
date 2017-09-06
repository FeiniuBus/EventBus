using EventBus.Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Core.Internal
{
    public class FailureInvoker : IInvoker
    {
        private readonly IServiceScope _serviceScope;
        private readonly IServiceProvider _serviceProvider;
        private readonly FailureHandleOptions _failureOptions;

        public MessageContext Context { get; }

        public FailureInvoker(IServiceProvider serviceProvider
            , MessageContext context)
        {
            _serviceScope = serviceProvider.CreateScope();
            _serviceProvider = _serviceScope.ServiceProvider;
            _failureOptions = _serviceProvider.GetRequiredService<FailureHandleOptions>();
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
            var method = handler.GetType().GetTypeInfo().GetMethod(nameof(IFailureHandler.HandleAsync));
            var task = method.Invoke(handler, new[] { message }) as Task<bool>;
            return task;
        }

        private object GetHandler(SubscribeInfo subscribeInfo)
        {
            return _serviceProvider.GetService(subscribeInfo.CallbackType);
        }

        private SubscribeInfo GetSubscribe()
        {
            return _failureOptions.DeadLetterInfos.FirstOrDefault(x => x.Exchange == Context.Exchange
                && x.Topic == Context.Topic
                && x.Group == Context.Queue);
        }

        private string GetMessage(SubscribeInfo subscribeInfo)
        {
            var message = Encoding.UTF8.GetString(Context.Content);
            return message;
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}

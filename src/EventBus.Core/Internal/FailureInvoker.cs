using EventBus.Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
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

            SetUpFailureContext();
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
            var receivedMsg = _serviceProvider.GetRequiredService<IMessageDecoder>().Decode(Context);
            return receivedMsg.Content;
        }

        private void SetUpFailureContext()
        {
            var accessor = _serviceProvider.GetRequiredService<IFailureContextAccessor>();
            var xDeath = Context.Args.BasicProperties.Headers["x-death"];
            var json = FeiniuBus.Util.FeiniuBusJsonConvert.SerializeObject(xDeath);
            var jarray = JArray.Parse(json);
            var jstr = (jarray[0]["queue"]).ToObject<string>();
            accessor.FailureContext = new ConsumeFailureContext
            {
                FailureGroup = jstr
            };
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}

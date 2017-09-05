using EventBus.Core.Infrastructure;
using EventBus.Core.Internal;

namespace EventBus.Core
{
    public class EventDescriptor
    {
        public EventDescriptor(string routeKey, IMessage message, string exchange = "default.exchange@feiniubus", object args = null)
        {
            RouteKey = routeKey;
            Message = message;
            ContentType = message.Content.GetType().FullName;
            Exchange = exchange;
            Arguments = args == null ? null : new AnonymousObject(args);
        }

        public string Exchange { get; }

        public string RouteKey { get; }

        public IMessage Message { get; }

        public string ContentType { get; }

        public AnonymousObject Arguments { get; set; }
    }
}

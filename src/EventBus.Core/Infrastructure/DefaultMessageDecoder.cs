using System.Text;
using Newtonsoft.Json.Linq;
using EventBus.Core;
using EventBus.Core.Internal.Model;

namespace EventBus.Subscribe.Infrastructure
{
    public class DefaultMessageDecoder : IMessageDecoder
    {
        public ReceivedMessage Decode(MessageContext context)
        {
            var json = Encoding.UTF8.GetString(context.Content);
            var jobject = JObject.Parse(json);

            return new ReceivedMessage
            {

            };
        }
    }
}

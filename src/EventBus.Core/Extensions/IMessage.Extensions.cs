using EventBus.Core.Infrastructure;
using Newtonsoft.Json;

namespace EventBus.Core.Extensions
{
    public static class IMessageExtensions
    {
        public static object GetTransferMessage(this IMessage message)
        {
            return message.Content;
        }

        public static string GetTransferJson(this IMessage message)
        {
            return FeiniuBus.Util.FeiniuBusJsonConvert.SerializeObject(message.GetTransferMessage());
        }
    }
}

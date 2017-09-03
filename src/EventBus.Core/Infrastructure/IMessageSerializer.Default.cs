using Newtonsoft.Json;
using System.Text;

namespace EventBus.Core.Infrastructure
{
    public class DefaultMessageSerializer : IMessageSerializer
    {
        public byte[] Serialize<MessageT>(MessageT message) where MessageT : class
        {
            var str = JsonConvert.SerializeObject(message);
            return Encoding.UTF8.GetBytes(str);
        }
    }
}

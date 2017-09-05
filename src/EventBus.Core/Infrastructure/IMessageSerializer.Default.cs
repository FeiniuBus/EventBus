using Newtonsoft.Json;
using System.Text;

namespace EventBus.Core.Infrastructure
{
    public class DefaultMessageSerializer : IMessageSerializer
    {
        public T Deserialize<T>(byte[] message)
        {
            var jsonStr = Encoding.UTF8.GetString(message);
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

        public byte[] Serialize(object message)
        {
            var jsonStr = JsonConvert.SerializeObject(message);
            return Encoding.UTF8.GetBytes(jsonStr);
        }
    }
}

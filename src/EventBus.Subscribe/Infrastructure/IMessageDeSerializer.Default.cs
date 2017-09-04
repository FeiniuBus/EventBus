using System;

namespace EventBus.Subscribe.Infrastructure
{
    public class DefaultMessageDeSerializer : IMessageDeSerializer
    {
        public MessageT Deserialize<MessageT>(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(byte[] bytes, Type type)
        {
            throw new NotImplementedException();
        }
    }
}

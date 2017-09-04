using System;

namespace EventBus.Subscribe
{
    public interface IMessageDeSerializer
    {
        MessageT Deserialize<MessageT>(byte[] bytes);
        object Deserialize(byte[] bytes, Type type);
    }
}

namespace EventBus.Core
{
    public interface IMessageSerializer
    {
        byte[] Serialize(object message);
        T Deserialize<T>(byte[] message);
    }
}

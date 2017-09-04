using RabbitMQ.Client;

namespace EventBus.Subscribe
{
    public class SubscribeContext
    {
        public string Name { get; set; }

        public string Key { get; set; }

        public string Queue { get; set; }

        public ulong DeliveryTag { get; set; }

        public IModel Channel { get; set; }

        public byte[] Content { get; set; }
    }
}

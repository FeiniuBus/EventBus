namespace EventBus.Subscribe
{
    public class SubscribeContext
    {
        public string Name { get; set; }

        public string Key { get; set; }

        public string Queue { get; set; }

        public long DeliveryTag { get; set; }
    }
}

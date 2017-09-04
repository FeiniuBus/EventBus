namespace EventBus.Subscribe
{
    public class SubscribeOptions
    {
        public string DefaultGroup { get; set; }

        public int ConsumerClientCount { get; set; } = 1;
    }
}

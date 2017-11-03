namespace EventBus.Core
{
    public class FailContext
    {
        public object State { get; set; }

        public string Raw { get; set; }

        public string ExceptionMessage { get; set; }
    }
}

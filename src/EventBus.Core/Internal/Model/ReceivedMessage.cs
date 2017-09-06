using EventBus.Core.State;
using System;

namespace EventBus.Core.Internal.Model
{
    public class ReceivedMessage
    {
        public long Id { get; set; }
        public long MessageId { get; set; }
        public long TransactId { get; set; }
        public string Group { get; set; }
        public string RouteKey { get; set; }
        public string MetaData { get; set; }
        public string Content { get; set; }
        public int Retries { get; set; }
        public DateTime ReceivedTime { get; set; }
        public DateTime ExpiredTime { get; set; }
        public MessageState State { get; set; }
    }
}

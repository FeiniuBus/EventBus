using EventBus.Core.Infrastructure;
using EventBus.Core.State;
using System;

namespace EventBus.Publish.Internal.Model
{
    public class Message
    {
        public long Id { get; set; }
        public long MessageId { get; set; }
        public long TransactId { get; set; }
        public string MetaData { get; set; }
        public string Content { get; set; }
        public string Exchange { get; set; }
        public string RouteKey { get; set; }
        public MessageType Type { get; set; }
        public MessageState State { get; set; }
        public DateTime CreationDate { get; set; }
    }
}

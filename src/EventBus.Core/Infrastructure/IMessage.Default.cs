using EventBus.Core.State;
using System;

namespace EventBus.Core.Infrastructure
{
    public class DefaultMessage : IMessage
    {
        public DefaultMessage(object content, MessageState state = MessageState.Processing)
        {
            MetaData = new MetaData();
            Content = content;
            State = state;
            CreateTime = DateTime.Now;
        }

        public IMetaData MetaData { get; }

        public object Content { get; }

        public DateTime CreateTime { get; }

        public MessageState State { get; }
    }
}

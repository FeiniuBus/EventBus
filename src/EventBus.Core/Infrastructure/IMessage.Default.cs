using EventBus.Core.State;
using System;

namespace EventBus.Core.Infrastructure
{
    public class DefaultMessage<TContent> : IMessage<TContent>
    {
        public DefaultMessage(TContent content, MessageState state = MessageState.Processing)
        {
            MetaData = new MetaData();
            Content = content;
            State = state;
            CreateTime = DateTime.Now;
        }

        public IMetaData MetaData { get; }

        public TContent Content { get; }

        public DateTime CreateTime { get; }

        public MessageState State { get; }
    }
}

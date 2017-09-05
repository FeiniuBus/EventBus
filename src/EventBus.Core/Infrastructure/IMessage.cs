using EventBus.Core.State;
using System;

namespace EventBus.Core.Infrastructure
{
    public interface IMessage
    {
        IMetaData MetaData { get; }
        object Content { get; }
        DateTime CreateTime { get; }
        MessageState State { get; }
    }
}

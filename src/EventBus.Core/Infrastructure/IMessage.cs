using EventBus.Core.State;
using System;

namespace EventBus.Core.Infrastructure
{
    public interface IMessage< TContent> 
    {
        IMetaData MetaData { get; }
        TContent Content { get; }
        DateTime CreateTime { get; }
        MessageState State { get; }
    }
}

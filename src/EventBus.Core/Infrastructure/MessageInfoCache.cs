using System;
using System.Collections.Concurrent;

namespace EventBus.Core.Infrastructure
{
    public class MessageInfoCache
    {
        private readonly ConcurrentDictionary<Type, MessageInfo> _cache = new ConcurrentDictionary<Type, MessageInfo>();

        public MessageInfo GetMessageInfo(Type type)
        {
            _cache.TryGetValue(type, out MessageInfo messageInfo);
            return messageInfo;
        }
    }
}

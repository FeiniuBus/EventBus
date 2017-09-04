using System;
using System.Reflection;
using System.Collections.Concurrent;

namespace EventBus.Core.Infrastructure
{
    public class MessageInfoCache
    {
        private readonly ConcurrentDictionary<Type, MessageInfo> _cache = new ConcurrentDictionary<Type, MessageInfo>();
        private readonly AssemblyVisitor _assemblyVisitor;

        public ConcurrentDictionary<Type, MessageInfo> Entries => _cache;

        public MessageInfoCache(AssemblyVisitor assemblyVisitor)
        {
            _assemblyVisitor = assemblyVisitor;
        }

        public MessageInfo GetMessageInfo(Type type)
        {
            _cache.TryGetValue(type, out MessageInfo messageInfo);
            return messageInfo;
        }

        private void Init()
        {
            var events = _assemblyVisitor.GetTypesOfAttribute<EventAttribute>();
            foreach(var @event in events)
            {
                if (!_cache.TryAdd(@event, GetMessageInfo(@event)))
                {
                    throw new InvalidOperationException($"duplicated name and key pair");
                }
            }
        }

        private MessageInfo GetmMessageInfo(Type type)
        {
            var eventAttribute = type.GetTypeInfo().GetCustomAttribute<EventAttribute>();
            return new MessageInfo
            {
                Name = eventAttribute.Name,
                Key = eventAttribute.Key,
                Type = type
            };
        }
    }
}

using EventBus.Core;
using EventBus.Core.Infrastructure;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace EventBus.Subscribe.Infrastructure
{
    public class SubscribeInfoCache
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, SubscribeInfo>> _cache
            = new ConcurrentDictionary<string, ConcurrentDictionary<string, SubscribeInfo>>();

        public ConcurrentDictionary<string, ConcurrentDictionary<string, SubscribeInfo>> Entries => _cache;

        private readonly AssemblyVisitor _assemblyVisitor;
        private readonly SubscribeOptions _subscribeOptions;
        private readonly RabbitOptions _rabbitOptions;

        public SubscribeInfoCache(AssemblyVisitor assemblyVisitor
            , IOptions<SubscribeOptions> subscribeOptions
            , IOptions<RabbitOptions> rabbitOptions)
        {
            _assemblyVisitor = assemblyVisitor;
            _subscribeOptions = subscribeOptions.Value;
            _rabbitOptions = rabbitOptions.Value;

            Init();
        }

        public SubscribeInfo GetSubscribeInfo(string name, string group)
        {
            if (!Entries.TryGetValue(name, out ConcurrentDictionary<string, SubscribeInfo> innerDict))
            {
                return null;
            }
            else if (!innerDict.TryGetValue(group, out SubscribeInfo subscribeInfo))
            {
                return null;
            }
            else
            {
                return subscribeInfo;
            }
        }

        private void Init()
        {
            var subHandlers = _assemblyVisitor.GetSubClassesOf(typeof(ISubscribeHandler<>));
            var infos = subHandlers.Select(GetSubscribeInfo).ToArray();

            foreach(var info in infos)
            {
                var innerDict = _cache.GetOrAdd(info.Name, new ConcurrentDictionary<string, SubscribeInfo>());
                innerDict.GetOrAdd(info.Group, info);
            }
        }

        private SubscribeInfo GetSubscribeInfo(Type type)
        {
            var innerType = type.GenericTypeArguments[0];
            var @event = innerType.GetTypeInfo().GetCustomAttribute<EventAttribute>();
            var group = type.GetTypeInfo().GetCustomAttribute<SubscribeAttribute>()?.Group 
                ?? _subscribeOptions.DefaultGroup
                ?? throw new InvalidOperationException("Group can not be null");

            return new SubscribeInfo
            {
                HandlerType = type,
                InnerType = innerType,
                BaseType = type.GetTypeInfo().BaseType,
                Name = @event?.Name ?? _rabbitOptions.DefaultExchangeName,
                Group = group,
                Key = @event?.Key
            };
        }
    }
}

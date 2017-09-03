using EventBus.Core;
using EventBus.Core.Infrastructure;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace EventBus.Subscribe.Infrastructure
{
    internal class SubscribeInfoCache
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, SubscribeInfo>> _cache
            = new ConcurrentDictionary<string, ConcurrentDictionary<string, SubscribeInfo>>();

        private readonly AssemblyVisitor _assemblyVisitor;
        private readonly SubscribeOptions _subscribeOptions;

        public SubscribeInfoCache(AssemblyVisitor assemblyVisitor
            , IOptions<SubscribeOptions> subscribeOptions)
        {
            _assemblyVisitor = assemblyVisitor;
            _subscribeOptions = subscribeOptions.Value;

            Init();
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
            var name = innerType.GetTypeInfo().GetCustomAttribute<EventAttribute>()?.Name ?? throw new InvalidOperationException("Name can not be null");
            var group = type.GetTypeInfo().GetCustomAttribute<SubscribeAttribute>()?.Group 
                ?? _subscribeOptions.DefaultGroup
                ?? throw new InvalidOperationException("Group can not be null");

            return new SubscribeInfo
            {
                HandlerType = type,
                InnerType = innerType,
                BaseType = type.GetTypeInfo().BaseType,
                Name = name,
                Group = group
            };
        }
    }
}

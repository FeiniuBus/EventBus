//using EventBus.Core;
//using EventBus.Core.Infrastructure;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;

//namespace EventBus.Subscribe.Infrastructure
//{
//    public class SubscribeInfoCache
//    {
//        private readonly ConcurrentDictionary<string, List<SubscribeInfo>> _cache
//            = new ConcurrentDictionary<string, List<SubscribeInfo>>();

//        public ConcurrentDictionary<string, List<SubscribeInfo>> Entries => _cache;

//        private readonly AssemblyVisitor _assemblyVisitor;
//        private readonly SubscribeOptions _subscribeOptions;
//        private readonly RabbitOptions _rabbitOptions;

//        public SubscribeInfoCache(AssemblyVisitor assemblyVisitor
//            , IOptions<SubscribeOptions> subscribeOptions
//            , IOptions<RabbitOptions> rabbitOptions)
//        {
//            _assemblyVisitor = assemblyVisitor;
//            _subscribeOptions = subscribeOptions.Value;
//            _rabbitOptions = rabbitOptions.Value;
//        }

//        public SubscribeInfo GetSubscribeInfo(string topic, string group)
//        {
//            if (!Entries.TryGetValue(group, out List<SubscribeInfo> items))
//            {
//                return null;
//            }
//            else 
//            {
//                return items.FirstOrDefault(x => x.Topic == topic);
//            }
//        }

//        private void Init()
//        {
//            var subHandlers = _assemblyVisitor.GetSubClassesOf(typeof(ISubscribeHandler<>));
//            var infos = subHandlers.Select(GetSubscribeInfo).ToArray();

//            foreach(var info in infos)
//            {
//                var innerDict = _cache.GetOrAdd(info.Name, new ConcurrentDictionary<string, SubscribeInfo>());
//                innerDict.GetOrAdd(info.Group, info);
//            }
//        }

//        internal SubscribeInfo AddSubscribeInfo(Type type)
//        {
//            var innerType = type.GenericTypeArguments[0];
//            var @event = innerType.GetTypeInfo().GetCustomAttribute<EventAttribute>();
//            var group = type.GetTypeInfo().GetCustomAttribute<SubscribeAttribute>()?.Group 
//                ?? _subscribeOptions.DefaultGroup
//                ?? throw new InvalidOperationException("Group can not be null");

//            return new SubscribeInfo
//            {
//                HandlerType = type,
//                InnerType = innerType,
//                BaseType = type.GetTypeInfo().BaseType,
//                Name = @event?.Name ?? _rabbitOptions.DefaultExchangeName,
//                Group = group,
//                Key = @event?.Key
//            };
//        }
//    }
//}

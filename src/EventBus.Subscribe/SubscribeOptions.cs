using EventBus.Core;
using EventBus.Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using EventBus.Core.Util;

namespace EventBus.Subscribe
{
    public class SubscribeOptions
    {
        public string DefaultGroup { get; set; }

        public int ConsumerClientCount { get; set; } = 1;

        public IReadOnlyList<SubscribeInfo> SubscribeInfos { get; private set; }

        private readonly IList<SubscribeInfo> _subscribeInfos;

        public SubscribeOptions()
        {
            _subscribeInfos = new List<SubscribeInfo>();
        }

        public void RegisterCallback(string topic, Type callbackType)
        {
            if (_subscribeInfos.Any(x => x.Topic == topic && x.Group == null))
            {
                throw new InvalidOperationException($"Duplicated subscribe topic={topic}");
            }

            _subscribeInfos.Add(new SubscribeInfo
            {
                Topic = topic,
                CallbackType = callbackType
            });
        }

        public void RegisterCallback(string topic, string group, Type callbackType)
        {
            if (_subscribeInfos.Any(x => x.Topic == topic && x.Group == group))
            {
                throw new InvalidOperationException($"Duplicated subscribe topic={topic} group={group}");
            }

            _subscribeInfos.Add(new SubscribeInfo
            {
                Topic = topic,
                Group = group,
                CallbackType = callbackType
            });
        }

        public void RegisterCallback(string topic, string group, string exchange, Type callbackType)
        {
            if (_subscribeInfos.Any(x => x.Topic == topic && x.Group == group))
            {
                throw new InvalidOperationException($"Duplicated subscribe topic={topic} group={group}");
            }

            _subscribeInfos.Add(new SubscribeInfo
            {
                Exchange = exchange,
                Topic = topic,
                Group = group,
                CallbackType = callbackType
            });
        }

        public void Build(IServiceProvider serviceProvider)
        {
            var concator = serviceProvider.GetRequiredService<IEnviromentNameConcator>();
            var rabbitOptions = serviceProvider.GetRequiredService<RabbitOptions>();

            var defaultExchange = rabbitOptions.DefaultExchangeName;

            foreach(var info in _subscribeInfos)
            {
                if (string.IsNullOrEmpty(info.Exchange))
                {
                    info.Exchange = concator.Concat(info.Exchange ?? defaultExchange);
                    info.Group = concator.Concat(info.Group ?? DefaultGroup);
                }
            }

            SubscribeInfos = new List<SubscribeInfo>(_subscribeInfos);
        }
    }
}

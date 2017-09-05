using EventBus.Core;
using EventBus.Subscribe.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBus.Subscribe
{
    public class SubscribeOptions
    {
        public string DefaultGroup { get; set; }

        public int ConsumerClientCount { get; set; } = 1;

        public string DefaultExchange => "default.exchange@feiniubus";

        public IList<SubscribeInfo> SubscribeInfos { get; } = new List<SubscribeInfo>();

        public void RegisterExternalCallback(string topic, Type callbackType)
        {
            if (SubscribeInfos.Any(x => x.Topic == topic && x.Group == DefaultGroup))
            {
                throw new InvalidOperationException($"Duplicated subscribe topic={topic} group={DefaultGroup}");
            }

            SubscribeInfos.Add(new SubscribeInfo
            {
                Exchange = DefaultExchange,
                Topic = topic,
                Group = DefaultGroup,
                CallbackType = callbackType
            });
        }

        public void RegisterExternalCallback(string topic, string group, Type callbackType)
        {
            if (SubscribeInfos.Any(x => x.Topic == topic && x.Group == group))
            {
                throw new InvalidOperationException($"Duplicated subscribe topic={topic} group={group}");
            }

            SubscribeInfos.Add(new SubscribeInfo
            {
                Exchange = DefaultExchange,
                Topic = topic,
                Group = group,
                CallbackType = callbackType
            });
        }

        public void RegisterExternalCallback(string topic, string group, string exchange, Type callbackType)
        {
            if (SubscribeInfos.Any(x => x.Topic == topic && x.Group == group))
            {
                throw new InvalidOperationException($"Duplicated subscribe topic={topic} group={group}");
            }

            SubscribeInfos.Add(new SubscribeInfo
            {
                Exchange = exchange,
                Topic = topic,
                Group = group,
                CallbackType = callbackType
            });
        }
    }
}

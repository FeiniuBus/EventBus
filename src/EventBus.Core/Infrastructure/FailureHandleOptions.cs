using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EventBus.Core.Infrastructure
{
    public class FailureHandleOptions
    {
        public IReadOnlyCollection<SubscribeInfo> DeadLetterInfos { get; private set; }

        private IList<SubscribeInfo> _subscribeInfos;

        public FailureHandleOptions()
        {
            _subscribeInfos = new List<SubscribeInfo>();
        }

        public void RegisterFailureCallback(string topic, Type callbackType)
        {
            foreach (var info in _subscribeInfos)
            {
                if (info.Topic == topic)
                {
                    throw new InvalidOperationException($"Duplicate failure callback {topic}");
                }
            }

            _subscribeInfos.Add(new SubscribeInfo
            {
                Topic = topic,
                CallbackType = callbackType
            });
        }

        public void RegisterFailureCallback(string topic, string selfExchange, Type callbackType)
        {
            _subscribeInfos.Add(new SubscribeInfo
            {
                Group = DeadLetterQueueName(selfExchange),
                Topic = topic,
                CallbackType = callbackType
            });
        }

        public void BuildWithDefaultSelfExchangeName(string selfExchangeName, string deadLetterExchange)
        {
            foreach(var info in _subscribeInfos)
            {
                if (info.Group == null) info.Group = DeadLetterQueueName(selfExchangeName);
                info.Exchange = deadLetterExchange;

                if (_subscribeInfos.Count(x => x.Group == info.Group && x.Topic == info.Topic && x.Exchange == info.Topic) > 1)
                {
                    throw new InvalidOperationException($"Dulicate {info.Exchange} {info.Topic} {info.Group}");
                }
            }

            DeadLetterInfos = new ReadOnlyCollection<SubscribeInfo>(_subscribeInfos);
        }

        private string DeadLetterQueueName(string selfExchange) => "deadletter." + selfExchange;
    }
}

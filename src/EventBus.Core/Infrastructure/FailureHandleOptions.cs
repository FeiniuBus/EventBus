using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EventBus.Core.Infrastructure
{
    public class FailureHandleOptions
    {
        public IReadOnlyCollection<DeadLetterInfo> DeadLetterInfos { get; private set; }

        public void RegisterFailureCallback(string topic, Type callbackType)
        {
            var deadLetterInfos = DeadLetterInfos?.ToList() ?? new List<DeadLetterInfo>();
            foreach (var info in DeadLetterInfos)
            {
                if (info.Topic == topic)
                {
                    throw new InvalidOperationException($"Duplicate failure callback {topic}");
                }
            }

            deadLetterInfos.Add(new DeadLetterInfo
            {
                Topic = topic,
                HandlerType = callbackType
            });

            DeadLetterInfos = new ReadOnlyCollection<DeadLetterInfo>(deadLetterInfos);
        }
    }
}

using System;

namespace EventBus.Core.Infrastructure
{
    public class DeadLetterInfo
    {
        public string Topic { get; set; }

        public Type HandlerType { get; set; }
    }
}

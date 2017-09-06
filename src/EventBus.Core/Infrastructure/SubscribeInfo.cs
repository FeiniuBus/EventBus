using System;

namespace EventBus.Core.Infrastructure
{
    public class SubscribeInfo
    {
        public string Exchange { get; set; }

        public string Topic { get; set; }

        public string Group { get; set; }

        public Type CallbackType { get; set; }
    }
}

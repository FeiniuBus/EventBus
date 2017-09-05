using System;

namespace EventBus.Subscribe.Infrastructure
{
    public class SubscribeInfo
    {
        public Type HandlerType { get; set; }

        public Type InnerType { get; set; }

        public Type BaseType { get; set; }

        public string Name { get; set; }

        public string Group { get; set; }

        public string Key { get; set; }
    }
}

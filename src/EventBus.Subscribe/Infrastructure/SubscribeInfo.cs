using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Subscribe.Infrastructure
{
    internal class SubscribeInfo
    {
        public Type HandlerType { get; set; }

        public Type InnerType { get; set; }

        public Type BaseType { get; set; }

        public string Name { get; set; }

        public string Group { get; set; }
    }
}

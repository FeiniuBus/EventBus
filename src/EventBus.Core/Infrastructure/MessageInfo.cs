using System;

namespace EventBus.Core.Infrastructure
{
    public class MessageInfo
    {
        public Type Type { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }
    }
}

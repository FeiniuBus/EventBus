using System;

namespace EventBus.Core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SubscribeAttribute : Attribute
    {
        public string Name { get; set; } = null;
        public string Key { get; set; }
    }
}

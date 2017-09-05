using System;

namespace EventBus.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventAttribute: Attribute
    {
        public string Name { get; set; } = null;

        public string Key { get; set; }
    }
}

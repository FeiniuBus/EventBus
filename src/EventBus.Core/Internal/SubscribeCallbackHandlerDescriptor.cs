using System;

namespace EventBus.Core.Internal
{
    internal class SubscribeCallbackHandlerDescriptor
    {
        public SubscribeCallbackHandlerDescriptor(Type type, string key, string name = null)
        {
            Type = type;
            Name = name;
            Key = key;
        }

        internal Type Type { get; }
        internal string Name { get; }
        internal string Key { get; }
    }
}

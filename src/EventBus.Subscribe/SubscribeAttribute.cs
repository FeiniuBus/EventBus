using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Subscribe
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SubscribeAttribute: Attribute
    {
        public string Group { get; set; }
    }
}

using System;
using System.Reflection;

namespace EventBus.Core.Internal
{
    public class AnonymousPropertyInfo
    {
        public string MemberName { get; internal set; }
        public Type DeclaringType { get; internal set; }
        public MemberTypes MemberType { get; internal set; }
    }
}

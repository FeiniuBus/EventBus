using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EventBus.Core.Infrastructure
{
    public static class TypeExtenssion
    {
        public static bool IsChildClassOfInterface(this Type type, Type interfaceType)
        {
            if (!interfaceType.GetTypeInfo().IsInterface)
                throw new InvalidOperationException($"{nameof(interfaceType)} must be an interface");

            return type.GetInterfaces().Any(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == interfaceType);
        }
    }
}

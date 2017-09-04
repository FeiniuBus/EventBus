using System;
using System.Linq;
using System.Reflection;

namespace EventBus.Core.Infrastructure
{
    public static class AssemblyVisitorExtenssion
    {
        public static Type[] GetSubClassesOf(this AssemblyVisitor @this, Type type)
        {
            return @this.Classes.Where(cls => cls.GetTypeInfo().IsSubclassOf(type)).ToArray();
        }

        public static Type[] GetTypesOfAttribute<AttributeT>(this AssemblyVisitor @this) where AttributeT : Attribute
        {
            var types = @this.Classes.Where(cls => cls.GetTypeInfo().GetCustomAttribute<AttributeT>() != null).ToArray();
            return types;
        }
    }
}

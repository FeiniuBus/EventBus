using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace EventBus.Core.Internal
{
    public class AnonymousObject
    {
        private readonly object _instance;
        private readonly Type _type;

        public AnonymousObject(object instance)
        {
            _instance = instance;
            _type = instance.GetType();
        }

        public T GetValue<T>(string memberName)
        {
            var callSiteBinder = Binder.Convert(CSharpBinderFlags.None, typeof(T), _type);
            var typeDeclarer = CallSite<Func<CallSite, object, T>>.Create(callSiteBinder);

            Func<CallSite, object, T> typeDeclarerTarget = typeDeclarer.Target;
           
            var memberFinder = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, memberName, _type, new CSharpArgumentInfo[]
                {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
                }));

            return typeDeclarerTarget(typeDeclarer, memberFinder.Target(memberFinder, _instance));
        }

        public object GetValue(Type t, string memberName)
        {
            var callSiteBinder = Binder.Convert(CSharpBinderFlags.None, t, _type);
            var typeDeclarer = CallSite<Func<CallSite, object, object>>.Create(callSiteBinder);

            Func<CallSite, object, object> typeDeclarerTarget = typeDeclarer.Target;

            var memberFinder = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, memberName, _type, new CSharpArgumentInfo[]
                {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
                }));

            return typeDeclarerTarget(typeDeclarer, memberFinder.Target(memberFinder, _instance));
        }

        public IEnumerable<AnonymousMemberInfo> GetMembers()
        {
            var memberinfos = _type.GetMembers().Select(member => new AnonymousMemberInfo
            {
                MemberName = member.Name,
                DeclaringType = member.DeclaringType,
                MemberType = member.MemberType
            });
            return memberinfos;
        }
    }
}

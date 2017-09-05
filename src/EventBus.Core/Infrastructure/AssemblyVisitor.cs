using System;
using System.Reflection;
using System.Collections.Generic;

namespace EventBus.Core.Infrastructure
{
    public class AssemblyVisitor
    {
        public IList<Type> Classes { get; } = new List<Type>();

        public void Start()
        {
            foreach(var type in Assembly.GetEntryAssembly().ExportedTypes)
            {
            }
        }
    }
}

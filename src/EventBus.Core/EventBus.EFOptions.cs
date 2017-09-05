using System;

namespace EventBus.Core
{
    public class EventBusEFOptions
    {
        public EventBusEFOptions(Type dbContextType, string connectionString)
        {
            DbContextType = dbContextType;
            ConnectionString = connectionString;
        }

        public Type DbContextType { get; }

        public string ConnectionString { get; }
    }
}

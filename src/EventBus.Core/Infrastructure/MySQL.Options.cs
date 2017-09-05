using System;

namespace EventBus.Core.Infrastructure
{
    public class EventBusMySQLOptions
    {
        public EventBusMySQLOptions() { }

        public EventBusMySQLOptions(Type dbContextType)
        {
            DbContextType = dbContextType;
        }

        public EventBusMySQLOptions(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public Type DbContextType { get; set; }

        public string ConnectionString { get; set; }
    }
}

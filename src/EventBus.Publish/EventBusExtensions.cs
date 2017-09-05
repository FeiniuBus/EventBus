using EventBus.Core;
using EventBus.Publish;
using Microsoft.EntityFrameworkCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventBusExtensions
    {
        public static EventBusOptions UseEntityframework<TDbContext>(this EventBusOptions eventBusOptions)
            where TDbContext : DbContext
        {
            var extension = new EventBusMySQLExtension((provider, options) =>
            {
                options.DbContextType = typeof(TDbContext);
                var dbContext = provider.GetRequiredService<TDbContext>();
                var conn = dbContext.Database.GetDbConnection();
                var connectionString = conn.ConnectionString;
                if(conn.State != System.Data.ConnectionState.Closed)
                {
                    conn.Close();
                }
                options.ConnectionString = connectionString;
            });
            eventBusOptions.RegisterExtension(extension);
            return eventBusOptions;
        }

        public static EventBusOptions UseMySQL(this EventBusOptions eventBusOptions, string connectionString)
        {
            var extension = new EventBusMySQLExtension((provider, options) => options.ConnectionString = connectionString);
            eventBusOptions.RegisterExtension(extension);
            return eventBusOptions;
        }

        public static EventBusOptions UseRabbitMQ(this EventBusOptions eventBusOptions, Action<EventBus.Core.Infrastructure.RabbitOptions> configure)
        {
            var extension = new RabbitExtension(configure);
            eventBusOptions.RegisterExtension(extension);
            return eventBusOptions;
        }
    }
}

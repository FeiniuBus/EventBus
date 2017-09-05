using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace EventBus.Core
{
    public interface IEventTransaction :  IDisposable
    {
        IDbConnection Connection { get; }
        IsolationLevel IsolationLevel { get; }

        Task CommitAsync();
        Task RollbackAsync();
        IDbTransaction Transaction { get; }
        long TransactID { get; }
        Task PublishEventsAsync(IEnumerable<EventDescriptor> eventDescriptors);
    }
}

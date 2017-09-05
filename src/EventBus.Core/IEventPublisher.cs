using System;
using System.Data;
using System.Threading.Tasks;

namespace EventBus.Core
{
    public interface IEventPublisher : IDisposable
    {
        IEventPublisher CreateScope();
        Task PrepareAsync(EventDescriptor descriptor);
        Task PrepareAsync(EventDescriptor descriptor, IDbConnection dbConnection, IDbTransaction dbTransaction);
        Task ConfirmAsync();
        Task RollbackAsync();
    }
}

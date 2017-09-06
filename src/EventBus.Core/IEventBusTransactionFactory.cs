using System.Data;
using System.Threading.Tasks;

namespace EventBus.Core
{
    public interface IEventBusTransactionFactory
    {
        Task<IEventTransaction> BeginTransactionAsync(IDbConnection dbConnection, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}

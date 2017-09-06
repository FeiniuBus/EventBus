using EventBus.Core.State;
using System.Data;
using System.Threading.Tasks;

namespace EventBus.Core
{
    public interface IReceivedEventPersistenter
    {
        Task EnsureCreated();

        Task InsertAsync(object message, IDbConnection dbConnection, IDbTransaction dbTransaction);
        Task ChangeStateAsync(long messageId, long transactId, string group, MessageState messageState, IDbConnection dbConnection, IDbTransaction dbTransaction);
        Task ChangeStateAsync(long id, MessageState messageState, IDbConnection dbConnection, IDbTransaction dbTransaction);
    }
}

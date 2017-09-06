using EventBus.Core.State;
using System.Threading.Tasks;

namespace EventBus.Core
{
    public interface IReceivedEventPersistenter
    {
        Task EnsureCreatedAsync();

        Task InsertAsync(object message);
        Task ChangeStateAsync(long messageId, long transactId, string group, MessageState messageState);
        Task ChangeStateAsync(long id, MessageState messageState);
    }
}

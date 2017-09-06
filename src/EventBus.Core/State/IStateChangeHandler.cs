using EventBus.Core.Infrastructure;
using System.Threading.Tasks;

namespace EventBus.Core.State
{
    public interface IStateChangeHandler
    {
        bool CanHandle(MessageType messageType, string content, IMetaData metaData, StateChangedArgs args);

        Task HandleAsync(MessageType messageType, string content, IMetaData metaData, StateChangedArgs args);
    }
}

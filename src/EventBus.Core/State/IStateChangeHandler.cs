using EventBus.Core.Infrastructure;
using System;
using System.Threading.Tasks;

namespace EventBus.Core.State
{
    public interface IStateChangeHandler
    {
        bool CanHandle(Type contentType, string content, IMetaData metaData, StateChangedArgs args);

        Task HandleAsync(Type contentType, string content, IMetaData metaData, StateChangedArgs args);
    }
}

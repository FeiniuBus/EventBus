using System.Threading.Tasks;
using EventBus.Core.Internal.Model;

namespace EventBus.Core
{
    public interface ISubFailureHandler
    {
        Task HandleAsync(ReceivedMessage context);
    }
}

using System.Threading.Tasks;
using EventBus.Core;
using EventBus.Sample.Events;

namespace EventBus.Sample.FailedEventHandlers
{
    public class NewUserFailedMessageHandler : IFailedMessageHandler<NewUserEvent>
    {
        public Task HandleAsync(NewUserEvent message)
        {
            throw new System.NotImplementedException();
        }
    }
}

using System.Threading.Tasks;
using EventBus.Sample.Events;
using EventBus.Publish;

namespace EventBus.Sample.FailedEventHandlers
{
    public class NewUserFailedMessageHandler : IFailureHandler<NewUserEvent>
    {
        public Task HandleAsync(NewUserEvent message)
        {
            throw new System.NotImplementedException();
        }
    }
}

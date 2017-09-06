using System.Threading.Tasks;
using EventBus.Sample.Events;
using EventBus.Publish;

namespace EventBus.Sample.FailedEventHandlers
{
    public class NewUserFailedMessageHandler : IFailureHandler
    {
        public Task<bool> HandleAsync(string message)
        {
            throw new System.NotImplementedException();
        }
    }
}

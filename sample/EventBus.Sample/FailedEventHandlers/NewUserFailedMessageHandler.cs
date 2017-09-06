using System.Threading.Tasks;
using EventBus.Sample.Events;
using EventBus.Publish;
using EventBus.Core;

namespace EventBus.Sample.FailedEventHandlers
{
    public class NewUserFailedMessageHandler : IFailureHandler
    {
        public Task<bool> HandleAsync(string message)
        {
            return Task.FromResult(true);
        }
    }
}

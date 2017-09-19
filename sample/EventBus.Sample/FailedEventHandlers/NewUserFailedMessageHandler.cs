using System.Threading.Tasks;
using EventBus.Core;

namespace EventBus.Sample.FailedEventHandlers
{
    public class NewUserFailedMessageHandler : IFailureHandler
    {
        private readonly FailureContext _context;

        public NewUserFailedMessageHandler(IFailureContextAccessor accessor)
        {
            _context = accessor.FailureContext;
        }

        public Task<bool> HandleAsync(string message)
        {
            return Task.FromResult(false);
        }
    }
}

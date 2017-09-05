using System;
using System.Threading.Tasks;
using EventBus.Subscribe;

namespace EventBus.Sample.EventHandlers
{
    public class NewUserEventHandler : ISubscribeHandler
    {
        public Task<bool> HandleAsync(string message)
        {
            return Task.FromResult(true);
        }
    }
}

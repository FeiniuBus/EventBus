using System;
using System.Threading.Tasks;
using EventBus.Subscribe;

namespace EventBus.Sample.EventHandlers
{
    public class NewUserEventHandler : ISubscribeHandler
    {
        public Task<bool> HandleAsync(string message)
        {
            if (message == "123456")
                return Task.FromResult(false);
            return Task.FromResult(true);
        }
    }
}

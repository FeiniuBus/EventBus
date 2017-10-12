using System.Threading.Tasks;
using EventBus.Subscribe;
using Microsoft.AspNetCore.Hosting;

namespace EventBus.Sample.EventHandlers
{
    public class NewUserEventHandler : ISubscribeHandler
    {
        public NewUserEventHandler(IHostingEnvironment env)
        {

        }

        public Task<bool> HandleAsync(string message)
        {
            return Task.FromResult(false);
        }
    }
}

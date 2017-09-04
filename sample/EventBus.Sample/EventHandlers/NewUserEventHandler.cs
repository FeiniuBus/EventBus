using System;
using System.Threading.Tasks;
using EventBus.Core;
using EventBus.Sample.Events;
using EventBus.Subscribe;

namespace EventBus.Sample.EventHandlers
{
    public class NewUserEventHandler : ISubscribeHandler<NewUserEvent>
    {
        public Task HandleAsync(NewUserEvent message)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Threading.Tasks;
using EventBus.Core;
using EventBus.Sample.Events;

namespace EventBus.Sample.EventHandlers
{
    public class NewUserEventHandler : IMessageHandler<NewUserEvent>
    {
        public Task HandleAsync(NewUserEvent message)
        {
            throw new NotImplementedException();
        }
    }
}

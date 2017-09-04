using System;
using System.Threading.Tasks;

namespace EventBus.Subscribe.Infrastructure
{
    public interface IConsumerInvoker: IDisposable
    {
        Task InvokeAsync();
    }
}

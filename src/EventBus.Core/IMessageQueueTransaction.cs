using System;
using System.Threading.Tasks;

namespace EventBus.Core
{
    public interface IMessageQueueTransaction : IDisposable
    {
        Task CommitAsync();
        Task RollbackAsync();
        Task PublishAsync(string exchange, string routingKey, byte[] body);
    }
}

using System.Threading.Tasks;

namespace EventBus.Subscribe
{
    public interface ISubscribeHandler
    {
        Task<bool> HandleAsync(string message);
    }
}

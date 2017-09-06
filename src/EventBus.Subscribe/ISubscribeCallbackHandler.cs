using System.Threading.Tasks;

namespace EventBus.Subscribe
{
    public interface ISubscribeCallbackHandler
    {
        Task HandleAsync(string content);
    }
}

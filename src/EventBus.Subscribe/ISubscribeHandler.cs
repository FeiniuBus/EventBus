using System.Threading.Tasks;

namespace EventBus.Subscribe
{
    public interface ISubscribeHandler<MessageT> where MessageT : class
    {
        Task HandleAsync(MessageT message);
    }
}

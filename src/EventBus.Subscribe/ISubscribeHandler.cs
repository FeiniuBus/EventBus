using System.Threading.Tasks;

namespace EventBus.Subscribe
{
    public interface ISubscribeHandler<MT> where MT : class
    {
        Task HandleAsync(MT message);
    }
}

using System.Threading.Tasks;

namespace EventBus.Core
{
    public interface IMessageHandler<MT> where MT : class
    {
        Task HandleAsync(MT message);
    }
}

using System.Threading.Tasks;

namespace EventBus.Core
{
    public interface IFailedMessageHandler<MT> where MT : class
    {
        Task HandleAsync(MT message);
    }
}

using System.Threading.Tasks;

namespace EventBus.Publish
{
    public interface IFailureHandler<MT> where MT : class
    {
        Task HandleAsync(MT message);
    }
}

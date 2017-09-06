using System.Threading.Tasks;

namespace EventBus.Publish
{
    public interface IFailureHandler
    {
        Task<bool> HandleAsync(string message);
    }
}

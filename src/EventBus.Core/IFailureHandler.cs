using System.Threading.Tasks;

namespace EventBus.Core
{
    public interface IFailureHandler
    {
        Task<bool> HandleAsync(string message);
    }
}

using Microsoft.Extensions.DependencyInjection;

namespace EventBus.Core
{
    public interface IEventBusOptionsExtension
    {
        void AddServices(IServiceCollection serviceCollection);
    }
}

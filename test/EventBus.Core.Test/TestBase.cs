using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventBus.Core.Test
{
    public abstract class TestBase
    {
        protected readonly IServiceProvider Provider;

        protected TestBase()
        {
            var services = new ServiceCollection();
            // ReSharper disable once VirtualMemberCallInConstructor
            OnServiceProviderBuilding(services);
            Provider = services.BuildServiceProvider();
        }

        protected abstract void OnServiceProviderBuilding(IServiceCollection services);
    }
}

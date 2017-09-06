using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

namespace EventBus.Core
{
    public static class ApplicationExtenssion
    {
        public static IApplicationBuilder UseEventBus(this IApplicationBuilder appBuilder)
        {
            var services = appBuilder.ApplicationServices;

            var bootstrapper = services.GetService<IBootstrapper>();
            bootstrapper.Start();

            var lifeTime = services.GetService<IApplicationLifetime>();
            lifeTime.ApplicationStopped.Register(() =>
            {
                bootstrapper.Dispose();
            });

            return appBuilder;
        }
    }
}

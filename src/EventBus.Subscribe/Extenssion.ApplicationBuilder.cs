using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.Subscribe
{
    public static class ApplicationBuilderExtenssion
    {
        public static IApplicationBuilder UseSub(this IApplicationBuilder appBuilder)
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

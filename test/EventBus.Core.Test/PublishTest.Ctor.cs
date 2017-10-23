using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace EventBus.Core.Test
{
    public partial class PublishTest : TestBase
    {
        protected override void OnServiceProviderBuilding(IServiceCollection services)
        {
            IHostingEnvironment env = Substitute.For<IHostingEnvironment>();
            env.EnvironmentName = "staging";
            services.AddSingleton(env);
            services.AddDbContext<TestDbContext>(opt => opt.UseMySql(Consts.DbConnectionString));
            services.AddEventBus((opt =>
            {
                opt.UseEntityframework<TestDbContext>();
                opt.UseRabbitMQ(cnf =>
                {
                    cnf.HostName = Consts.RabbitHost;
                    cnf.UserName = Consts.RabbitUser;
                    cnf.Password = Consts.RabbitPassword;
                });
            }));
        }
    }
}

using EventBus.Sample.EventHandlers;
using EventBus.Subscribe;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using EventBus.Sample.FailedEventHandlers;
using EventBus.Core;
using EventBus.Alert;

namespace EventBus.Sample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        private const string ConnectionString = "Server=localhost;Port=3306;Database=FeiniuCAP; User=root;Password=123456;charset=UTF-8";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddDbContext<SampleDbContext>(options => options.UseMySql(ConnectionString));

            services.AddEventBus(options =>
            {
                options.UseEntityframework<SampleDbContext>();
                options.UseRabbitMQ(rabbit =>
                {
                    rabbit.HostName = "localhost";
                    rabbit.UserName = "guest";
                    rabbit.Password = "123456";
                    rabbit.Port = 5672;
                });

                options.UseFailureHandle(failure =>
                {
                    failure.RegisterFailureCallback("eventbus.testtopic", typeof(NewUserFailedMessageHandler));
                });
            });


            services.AddSub(options =>
            {
                options.ConsumerClientCount = 5;
                options.DefaultGroup = "FeiniuBusPayment1111";

                options.RegisterCallback("charge.ok.shuttle", "shuttle", typeof(NewUserEventHandlerShuttle));
                options.RegisterCallback("charge.ok.commute", "commute", typeof(NewUserEventHandlerCommuter));
                options.RegisterCallback("charge.ok.*", "pay", typeof(NewUserEventHandlerAll));
            });

            services.AddEventBusAlert(opts =>
            {
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddDebug()
                .AddConsole(LogLevel.Information);

            app.UseEventBus();

            app.UseMvc();
        }
    }

    public class SampleDbContext : DbContext
    {
        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
        {

        }
    }
}

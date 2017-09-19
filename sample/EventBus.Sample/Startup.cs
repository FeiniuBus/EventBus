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
                });

                options.UseFailureHandle(failure =>
                {
                    failure.RegisterFailureCallback("eventbus.testtopic", typeof(NewUserFailedMessageHandler));
                });
            });


            services.AddSub(options =>
            {
                options.ConsumerClientCount = 1;
                options.DefaultGroup = "eventbus.testgroup";

                options.RegisterCallback("eventbus.testtopic", typeof(NewUserEventHandler));
                options.RegisterCallback("eventbus.testtopic", "eventbus.testgroup2", typeof(NewUserEventHandler));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

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

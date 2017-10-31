using EventBus.Alert;
using EventBus.Core;
using EventBus.Subscribe;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.NuGet.Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private const string ConnectionString = "Server=192.168.126.138;Port=3306;Database=FeiniuCAP; User=root;Password=kge2001;charset=UTF-8";
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddDbContext<SampleDbContext>(options => options.UseMySql(ConnectionString));

            services.AddEventBus(options =>
            {
                options.UseEntityframework<SampleDbContext>();
                options.UseRabbitMQ(rabbit =>
                {
                    rabbit.HostName = "192.168.126.138";
                    rabbit.UserName = "andrew";
                    rabbit.Password = "kge2001";
                    rabbit.Port = 5672;
                });

                options.UseFailureHandle(failure =>
                {
                    
                });
            });


            services.AddSub(options =>
            {
                options.ConsumerClientCount = 5;
                options.DefaultGroup = "FeiniuBusPayment1111";
                
                //options.RegisterCallback("eventbus.testtopic", "eventbus.testgroup2", typeof(NewUserEventHandler));
            });

            services.AddEventBusAlert(opts =>
            {
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
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

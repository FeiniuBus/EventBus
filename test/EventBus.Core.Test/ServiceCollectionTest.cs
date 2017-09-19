//using Microsoft.Extensions.DependencyInjection;
//using Xunit;
//using EventBus.Subscribe;
//using Microsoft.Extensions.Options;
//using EventBus.Core.Infrastructure;
//using System;
//using Microsoft.EntityFrameworkCore;


//namespace EventBus.Core.Test
//{
//    public class ServiceCollectionTest
//    {
//        [Fact]
//        public void DI()
//        {
//            var serviceProvider = GetServiceProviders();

//            var bootstrappers = serviceProvider.GetServices<IBootstrapper>();
//            Assert.Single(bootstrappers);

//            var failureHandleOptions = serviceProvider.GetServices<IOptions<FailureHandleOptions>>();
//            Assert.Single(failureHandleOptions);

//            var consumers = serviceProvider.GetServices<IConsumer>();
//            Assert.Single(consumers);

//            var failureContextAccessors = serviceProvider.GetServices<IFailureContextAccessor>();
//            Assert.Single(failureContextAccessors);

//            var messageSerializers = serviceProvider.GetServices<IMessageSerializer>();
//            Assert.Single(messageSerializers);

//            var rabbitOptions = serviceProvider.GetServices<IOptions<RabbitOptions>>();
//            Assert.Single(rabbitOptions);

//            var messageDecoders = serviceProvider.GetServices<IMessageDecoder>();
//            Assert.Single(messageDecoders);
//        }

//        private IServiceProvider GetServiceProviders()
//        {
//            var services = new ServiceCollection();

//            services.AddOptions();
//            services.AddLogging();

//            services.AddDbContext<SampleDbContext>(options =>
//            {
//                options.UseInMemoryDatabase("mem-db");
//            });

//            services.AddEventBus(options =>
//            {
//                options.UseEntityframework<SampleDbContext>();

//                options.UseRabbitMQ(rabbit =>
//                {
//                    rabbit.HostName = "localhost";
//                });

//                options.UseFailureHandle(failure =>
//                {
//                });
//            });

//            services.AddSub(options =>
//            {
//            });

//            var serviceProvider = services.BuildServiceProvider();

//            return serviceProvider;
//        }
//    }

//    public class SampleDbContext : DbContext
//    {
//        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
//        {

//        }
//    }
//}

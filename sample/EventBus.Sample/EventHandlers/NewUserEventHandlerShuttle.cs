using System;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Subscribe;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace EventBus.Sample.EventHandlers
{
    public class NewUserEventHandlerShuttle : ISubscribeHandler
    {
        private readonly ILogger<NewUserEventHandlerShuttle> _logger;

        public NewUserEventHandlerShuttle(IHostingEnvironment env
            , ILogger<NewUserEventHandlerShuttle> logger)
        {
            _logger = logger;
        }

        public Task<bool> HandleAsync(string message)
        {
            _logger.LogInformation($"receive message1 from NewUserEventHandlerShuttle {message} {DateTime.Now}");

            Thread.Sleep(60 * 1000);

            return Task.FromResult(true);
        }
    }
}

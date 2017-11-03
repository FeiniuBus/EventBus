using System;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Subscribe;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace EventBus.Sample.EventHandlers
{
    public class NewUserEventHandlerAll : ISubscribeHandler
    {
        private readonly ILogger<NewUserEventHandlerAll> _logger;

        public NewUserEventHandlerAll(IHostingEnvironment env
            , ILogger<NewUserEventHandlerAll> logger)
        {
            _logger = logger;
        }

        public Task<bool> HandleAsync(string message)
        {
            _logger.LogInformation($"receive message1 from NewUserEventHandlerAll {message} {DateTime.Now}");

            return Task.FromResult(true);
        }
    }
}

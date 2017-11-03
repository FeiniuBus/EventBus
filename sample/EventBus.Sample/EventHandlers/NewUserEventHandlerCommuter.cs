using System;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Subscribe;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace EventBus.Sample.EventHandlers
{
    public class NewUserEventHandlerCommuter : ISubscribeHandler
    {
        private readonly ILogger<NewUserEventHandlerCommuter> _logger;

        public NewUserEventHandlerCommuter(IHostingEnvironment env
            , ILogger<NewUserEventHandlerCommuter> logger)
        {
            _logger = logger;
        }

        public Task<bool> HandleAsync(string message)
        {
            _logger.LogInformation($"receive message1 from NewUserEventHandlerCommuter {message} {DateTime.Now}");

            return Task.FromResult(true);
        }
    }
}

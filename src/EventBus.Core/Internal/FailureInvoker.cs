using EventBus.Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Core.Internal
{
    public class FailureInvoker : IInvoker
    {
        private readonly IServiceScope _serviceScope;
        private readonly IServiceProvider _serviceProvider;
        private readonly FailureHandleOptions _subscribeOptions;

        public MessageContext Context { get; }

        public FailureInvoker(IServiceProvider serviceProvider
            , MessageContext context)
        {
            _serviceScope = serviceProvider.CreateScope();
            _serviceProvider = _serviceScope.ServiceProvider;
            _subscribeOptions = _serviceProvider.GetRequiredService<FailureHandleOptions>();
            Context = context;
        }

        public Task<bool> InvokeAsync()
        {
            throw new NotImplementedException();
        }
    }
}

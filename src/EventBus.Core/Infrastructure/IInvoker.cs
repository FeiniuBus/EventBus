using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Core.Infrastructure
{
    public interface IInvoker
    {
        Task<bool> InvokeAsync();

        
    }
}

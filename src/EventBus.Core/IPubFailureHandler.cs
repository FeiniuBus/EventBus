using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Core
{
    public interface IPubFailureHandler
    {
        Task HandleAsync(string exchange, string topick, byte[] content);
    }
}

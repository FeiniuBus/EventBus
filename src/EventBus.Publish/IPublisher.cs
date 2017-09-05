using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventBus.Publish
{
    public interface IPublisher: IDisposable
    {
        Task PublishAsync<MessageT>(MessageT message) where MessageT : class;
        Task PublishAsync<MessageT>(MessageT message, IDictionary<string, object> args) where MessageT : class;
<<<<<<< HEAD
   
=======
>>>>>>> parent of 32b76d2... add tx
    }
}

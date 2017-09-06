using System;

namespace EventBus.Core
{
    public interface IClient: IDisposable
    {
        Action<MessageContext> OnReceive { get; set; }

        void Subscribe(string[] topics);
        void Listening();
    }
}

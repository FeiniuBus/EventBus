using System;

namespace EventBus.Subscribe
{
    public interface ISubscribeClient: IDisposable
    {
        Action<SubscribeContext> OnReceive { get; set; }

        void Subscribe(string[] topics);
        void Listening();
    }
}

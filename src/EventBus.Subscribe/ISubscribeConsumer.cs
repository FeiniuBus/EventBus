using System;

namespace EventBus.Subscribe
{
    public interface ISubscribeConsumer: IDisposable
    {
        void Start();
    }
}

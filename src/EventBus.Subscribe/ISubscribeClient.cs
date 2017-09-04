using System;

namespace EventBus.Subscribe
{
    public interface ISubscribeClient: IDisposable
    {
        void Start();
        void Commit();
    }
}

using System;

namespace EventBus.Subscribe
{
    public interface IBootstrapper: IDisposable
    {
        void Start();
    }
}

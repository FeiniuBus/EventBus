using System;

namespace EventBus.Core
{
    public interface IBootstrapper: IDisposable
    {
        void Start();
    }
}

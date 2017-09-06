using System;

namespace EventBus.Core
{
    public interface IConsumer: IDisposable
    {
        void Start();
    }
}

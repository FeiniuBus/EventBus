using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Subscribe
{
    public interface IBootstrapper: IDisposable
    {
        void Start();
    }
}

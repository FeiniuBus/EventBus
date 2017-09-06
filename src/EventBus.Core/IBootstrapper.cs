using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Core
{
    public interface IBootstrapper: IDisposable
    {
        void Start();
    }
}

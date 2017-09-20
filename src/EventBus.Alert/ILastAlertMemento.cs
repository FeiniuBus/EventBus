using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Alert
{
    public interface ILastAlertMemento
    {
        DateTime LastAlert { get; set; }
    }
}

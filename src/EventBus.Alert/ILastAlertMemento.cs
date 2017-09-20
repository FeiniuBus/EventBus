using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Alert
{
    public interface ILastAlertMemento
    {
        DateTime LastPubAlert { get; set; }

        DateTime LastSubAlert { get; set; }
    }
}

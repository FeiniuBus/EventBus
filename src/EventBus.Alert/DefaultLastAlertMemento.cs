using System;

namespace EventBus.Alert
{
    public class DefaultLastAlertMemento : ILastAlertMemento
    {
        public DateTime LastAlert { get; set; }
    }
}

using System;

namespace EventBus.Alert
{
    public class DefaultLastAlertMemento : ILastAlertMemento
    {
        public DateTime LastPubAlert { get; set; }

        public DateTime LastSubAlert { get; set; }
    }
}

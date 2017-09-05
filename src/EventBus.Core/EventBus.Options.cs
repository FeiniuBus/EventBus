using JetBrains.Annotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EventBus.Core
{
    public class EventBusOptions
    {
        private readonly IList<IEventBusOptionsExtension> _extensions;

        public EventBusOptions()
        {
            _extensions = new List<IEventBusOptionsExtension>();
        }

        public IReadOnlyCollection<IEventBusOptionsExtension> Extensions => new ReadOnlyCollection<IEventBusOptionsExtension>(_extensions);

        /// <summary>
        /// Registers an extension that will be executed when building services.
        /// </summary>
        /// <param name="extension"></param>
        public void RegisterExtension([NotNull]IEventBusOptionsExtension extension)
        {
            _extensions.Add(extension);
        }
    }
}

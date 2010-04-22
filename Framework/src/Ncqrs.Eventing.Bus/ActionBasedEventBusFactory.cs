using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Ncqrs.Domain.Bus
{
    /// <summary>
    /// A simple event bus factory that uses a <see cref="Func{IEventBus}"/> to
    /// create the instance.
    /// </summary>
    public class MethodBasedEventBusFactory : IEventBusFactory
    {
        /// <summary>
        /// The method that creates the event bus.
        /// </summary>
        private readonly Func<IEventBus> _eventBusCreationMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodBasedEventBusFactory"/> class.
        /// </summary>
        /// <param name="eventBusCreationMethod">The event bus creation method.</param>
        public MethodBasedEventBusFactory(Func<IEventBus> eventBusCreationMethod)
        {
            Contract.Requires<ArgumentNullException>(eventBusCreationMethod != null, "The eventBusCreationMethod cannot be null.");

            _eventBusCreationMethod = eventBusCreationMethod;
        }

        public IEventBus CreateEventBus()
        {
            return _eventBusCreationMethod.Invoke();
        }
    }
}

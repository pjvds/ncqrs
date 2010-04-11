using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ncqrs.Eventing.Bus
{
    public class MethodBasedEventBusFactory : IEventBusFactory
    {
        private Func<IEventBus> _eventBusCreationMethod;

        public MethodBasedEventBusFactory(Func<IEventBus> eventBusCreationMethod)
        {
            _eventBusCreationMethod = eventBusCreationMethod;
        }

        public IEventBus CreateEventBus()
        {
            return _eventBusCreationMethod.Invoke();
        }
    }
}

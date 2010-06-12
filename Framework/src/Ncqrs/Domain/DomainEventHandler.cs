using System;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing;

namespace Ncqrs.Domain
{
    public abstract class DomainEventHandler<TEvent> : IDomainEventHandler where TEvent : IEvent
    {
        public abstract Boolean HandleEvent(TEvent evnt);

        Boolean IDomainEventHandler.HandleEvent(IEvent evnt)
        {
            Boolean handled = false;

            if (evnt is TEvent)
            {
                handled |= HandleEvent((TEvent)evnt);
            }

            return handled;
        }
    }
}

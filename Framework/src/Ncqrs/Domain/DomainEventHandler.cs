using System;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Domain
{
    public abstract class DomainEventHandler<TEvent> : IDomainEventHandler where TEvent : SourcedEvent
    {
        public abstract Boolean HandleEvent(TEvent evnt);

        Boolean IDomainEventHandler.HandleEvent(SourcedEvent evnt)
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

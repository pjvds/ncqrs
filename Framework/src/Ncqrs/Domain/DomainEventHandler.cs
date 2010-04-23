using System;
using Ncqrs.Eventing;

namespace Ncqrs.Domain
{
    public abstract class DomainEventHandler<TEvent> : IDomainEventHandler where TEvent : DomainEvent
    {
        public abstract Boolean HandleEvent(TEvent evnt);

        Boolean IDomainEventHandler.HandleEvent(DomainEvent evnt)
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

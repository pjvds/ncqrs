using System;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Domain
{
    public abstract class DomainEventHandler<TEvent> : IDomainEventHandler where TEvent : ISourcedEvent
    {
        public abstract Boolean HandleEvent(TEvent evnt);

        Boolean IDomainEventHandler.HandleEvent(ISourcedEvent evnt)
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

using System;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Domain
{
    public abstract class DomainEventHandler<TEvent> : IDomainEventHandler
    {
        public abstract Boolean HandleEvent(TEvent evnt);

        Boolean IDomainEventHandler.HandleEvent(ISourcedEvent<IEventData> evnt)
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

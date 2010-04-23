using System;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Domain
{
    public abstract class ExactDomainEventHandler<TEvent> : IDomainEventHandler where TEvent : DomainEvent
    {
        public abstract Boolean HandleEvent(TEvent evnt);

        Boolean IDomainEventHandler.HandleEvent(DomainEvent evnt)
        {
            Boolean handled = false;
            Type evntType = evnt.GetType();

            if(typeof(TEvent) == evntType)
            {
                handled |= HandleEvent((TEvent)evnt);
            }

            return handled;
        }
    }
}

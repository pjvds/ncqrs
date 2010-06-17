using System;

namespace Ncqrs.Eventing.Sourcing
{
    public abstract class SourcedEventHander<TEvent> : ISourcedEventHandler where TEvent : SourcedEvent
    {
        public abstract Boolean HandleEvent(TEvent evnt);

        Boolean ISourcedEventHandler.HandleEvent(SourcedEvent evnt)
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

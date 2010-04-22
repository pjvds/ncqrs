using System;

namespace Ncqrs.Domain
{
    public abstract class ExactInternalEventHandler<TEvent> : IInternalEventHandler where TEvent : IEvent
    {
        public abstract Boolean HandleEvent(TEvent evnt);

        Boolean IInternalEventHandler.HandleEvent(IEvent evnt)
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

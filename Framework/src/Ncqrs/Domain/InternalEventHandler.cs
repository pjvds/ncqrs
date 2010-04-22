using System;

namespace Ncqrs.Eventing.ServiceModel.Bus
{
    public abstract class InternalEventHandler<TEvent> : IInternalEventHandler where TEvent : IEvent
    {
        public abstract Boolean HandleEvent(TEvent evnt);

        Boolean IInternalEventHandler.HandleEvent(IEvent evnt)
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

using System;

namespace Ncqrs.Eventing.Sourcing
{
    //public abstract class SourcedEventHander<TEvent> : ISourcedEventHandler where TEvent : ISourcedEvent
    //{
    //    public abstract Boolean HandleEvent(TEvent evnt);

    //    Boolean ISourcedEventHandler.HandleEvent(ISourcedEvent evnt)
    //    {
    //        Boolean handled = false;

    //        if (evnt is TEvent)
    //        {
    //            handled |= HandleEvent((TEvent)evnt);
    //        }

    //        return handled;
    //    }
    //}
}

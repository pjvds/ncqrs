using System;

namespace Ncqrs.Eventing
{
    public delegate void EventsAcceptedEventHandler(EventSource sender, EventsAcceptedEventArgs e);
}

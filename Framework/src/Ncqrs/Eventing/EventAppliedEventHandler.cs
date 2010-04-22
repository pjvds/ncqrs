using System;

namespace Ncqrs.Eventing
{
    public delegate void EventAppliedEventHandler(EventSource sender, EventAppliedEventArgs e);
}

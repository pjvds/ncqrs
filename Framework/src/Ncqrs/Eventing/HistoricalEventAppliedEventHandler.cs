using System;

namespace Ncqrs.Eventing
{
    public delegate void HistoricalEventAppliedEventHandler(EventSource sender, HistoricalEventAppliedEventArgs e);
}

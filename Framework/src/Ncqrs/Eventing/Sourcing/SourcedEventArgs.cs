using System;

namespace Ncqrs.Eventing.Sourcing
{
    internal class EventAppliedArgs : EventArgs
    {
        public SourcedEvent AppliedEvent { get; private set; }

        public EventAppliedArgs(SourcedEvent appliedEvent)
        {
            AppliedEvent = appliedEvent;
        }
    }
}

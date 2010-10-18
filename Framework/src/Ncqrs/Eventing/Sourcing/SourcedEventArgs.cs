using System;

namespace Ncqrs.Eventing.Sourcing
{
    internal class EventAppliedArgs : EventArgs
    {
        public ISourcedEvent AppliedEvent { get; private set; }

        public EventAppliedArgs(ISourcedEvent appliedEvent)
        {
            AppliedEvent = appliedEvent;
        }
    }
}

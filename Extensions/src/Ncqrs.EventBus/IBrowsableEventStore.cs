using System;
using System.Collections.Generic;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus
{    
    public interface IBrowsableEventStore
    {
        IEnumerable<SourcedEvent> FetchEvents(int maxCount);
        void MarkLastProcessedEvent(SequencedEvent evnt);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus
{    
    public interface IEventStore
    {
        void SetCursorPositionAfter(Guid lastEventId);
        SequencedEvent GetNext();
        void UnblockSource(Guid eventSourceId);
    }    

    public interface IPipelineStateStore
    {
        void MarkLastProcessedEvent(SequencedEvent evnt);
        void EnqueueForLaterProcessing(SourcedEvent evnt);
        SourcedEvent Dequeue();
    }
}

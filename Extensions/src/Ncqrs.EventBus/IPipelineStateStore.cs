using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus
{
    public interface IPipelineStateStore
    {
        void MarkLastProcessedEvent(SequencedEvent evnt);
        void EnqueueForLaterProcessing(SourcedEvent evnt);
        SourcedEvent Dequeue();
    }
}
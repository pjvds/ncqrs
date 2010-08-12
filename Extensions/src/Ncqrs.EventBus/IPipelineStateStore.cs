using System;

namespace Ncqrs.EventBus
{
    public interface IPipelineStateStore
    {
        void MarkLastProcessedEvent(SequencedEvent evnt);
        Guid? GetLastProcessedEvent();
    }
}
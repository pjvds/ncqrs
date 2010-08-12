using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus
{
    public interface IPipelineBackupQueue
    {
        void EnqueueForLaterProcessing(SourcedEvent evnt);        
    }
}
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus.IntegrationTest
{
    public class InMemoryPipelineBackupQueue : IPipelineBackupQueue
    {
        public void EnqueueForLaterProcessing(SourcedEvent evnt)
        {            
        }

        public SourcedEvent Dequeue()
        {
            return null;
        }
    }
}
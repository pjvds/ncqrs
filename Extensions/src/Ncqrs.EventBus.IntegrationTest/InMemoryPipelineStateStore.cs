using System;

namespace Ncqrs.EventBus.IntegrationTest
{
    public class InMemoryPipelineStateStore : IPipelineStateStore
    {
        public void MarkLastProcessedEvent(SequencedEvent evnt)
        {
            Console.WriteLine("* Marking last processed event as {0}", evnt.Event.EventIdentifier);
        }

        public Guid? GetLastProcessedEvent()
        {
            return null;
        }
    }
}
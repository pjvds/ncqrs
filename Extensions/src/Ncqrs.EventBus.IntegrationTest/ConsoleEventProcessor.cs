using System;
using System.IO;
using System.Threading;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus.IntegrationTest
{
    public class ConsoleEventProcessor : IEventProcessor
    {
        public int ProcessedEvents;

        public void Process(SourcedEvent evnt)
        {
            Thread.Sleep(200);

            Interlocked.Increment(ref ProcessedEvents);

            Console.WriteLine("Processing event {0} (id {1})", evnt.EventSequence, evnt.EventIdentifier);
        }
    }
}
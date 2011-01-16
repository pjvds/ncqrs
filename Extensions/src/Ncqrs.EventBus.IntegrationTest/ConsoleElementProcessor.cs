using System;
using System.Threading;

namespace Ncqrs.EventBus.IntegrationTest
{
    public class ConsoleElementProcessor : IElementProcessor
    {
        public int ProcessedEvents;

        public void Process(IProcessingElement evnt)
        {
            Thread.Sleep(100);

            Interlocked.Increment(ref ProcessedEvents);

            Console.WriteLine("Processing event {0} (id {1})", evnt.SequenceNumber, evnt.UniqueId);
        }
    }
}
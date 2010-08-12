using System;
using System.Threading;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus.IntegrationTest
{
    public class ConsoleEventProcessor : IEventProcessor
    {
        public void Process(SourcedEvent evnt)
        {
            Thread.Sleep(300);
            Console.WriteLine("Processing event {0}", evnt.EventIdentifier);
        }
    }
}
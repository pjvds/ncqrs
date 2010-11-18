using System;
using System.Configuration;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Storage.SQL;

namespace Ncqrs.EventBus.IntegrationTest
{
    class Program
    {        
        static void Main(string[] args)
        {
            //GenerateEvents();
            //GenerateEventsForAggregateRoots();
            ProcessEvents();

            //Console.WriteLine("Press any key");

            //Console.ReadLine();
        }

        private static void ProcessEvents()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Main"].ConnectionString;

            var consoleEventProcessor = new ConsoleEventProcessor();
            var p = new Pipeline(
                consoleEventProcessor,                
                new MsSqlServerPipelineStateStore(connectionString),
                //new InMemoryPipelineStateStore(),
                new MsSqlServerBrowsableEventStore(new MsSqlServerEventStore(connectionString)), 
                //new FakeEventStore(),
                new ThresholdedEventFetchPolicy(10, 20));
            p.Start();
            Console.ReadLine();
            p.Stop();
            Console.WriteLine("Processed {0} events", consoleEventProcessor.ProcessedEvents);
            Console.ReadLine();

        }

        private static void GenerateEvents()
        {
            var eventStore = new MsSqlServerEventStore(ConfigurationManager.ConnectionStrings["Main"].ConnectionString);
            for (int i = 0; i < 1000; i++)
            {
                eventStore.SaveEvents(new[] { new RandomEvent(i) });
            }
        }

        private static void GenerateEventsForAggregateRoots()
        {
            const int aggregateRootCount = 10;
            var aggregateRoots = Enumerable.Range(0, aggregateRootCount).Select(x => Guid.NewGuid()).ToList();
            var random = new Random();

            var eventStore = new MsSqlServerEventStore(ConfigurationManager.ConnectionStrings["Main"].ConnectionString);
            for (int i = 0; i < 1000; i++)
            {
                Guid rootId = aggregateRoots[random.Next(aggregateRootCount)];
                eventStore.SaveEvents(new[] { new RandomEvent(rootId, i) });
            }
        }
    }
}

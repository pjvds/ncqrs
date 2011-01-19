using System;
using System.Configuration;
using System.Linq;
using System.Text;
using Ncqrs.Eventing;
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

            var consoleEventProcessor = new ConsoleElementProcessor();
            var p = Pipeline.Create(consoleEventProcessor, new MsSqlServerEventStoreElementStore(connectionString));
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
                var uncommittedEventStream = new UncommittedEventStream(Guid.NewGuid());
                uncommittedEventStream.Append(new UncommittedEvent(Guid.NewGuid(), Guid.NewGuid(), i, i, DateTime.Now, new object(), new Version(1, 0)));                
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

                var uncommittedEventStream = new UncommittedEventStream(Guid.NewGuid());
                uncommittedEventStream.Append(new UncommittedEvent(Guid.NewGuid(), rootId, i, i, DateTime.Now, new object(), new Version(1, 0)));                
            }
        }
    }
}

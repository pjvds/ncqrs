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

            var consoleEventProcessor1 = new ConsoleElementProcessor("1", 100);
            var consoleEventProcessor2 = new ConsoleElementProcessor("2", 200);
            var p1 = Pipeline.Create("First pipeline", consoleEventProcessor1,
                                     new MsSqlServerEventStoreElementStore(connectionString));
            var p2 = Pipeline.Create("Second pipeline", consoleEventProcessor2,
                                     new MsSqlServerEventStoreElementStore(connectionString));
            
            p1.Start();
            p2.Start();
            Console.ReadLine();
            p1.Stop();
            p2.Stop();
            Console.WriteLine("Processed {0} events", consoleEventProcessor1.ProcessedEvents);
            Console.ReadLine();

        }

        private static void GenerateEvents()
        {
            var eventStore = new MsSqlServerEventStore(ConfigurationManager.ConnectionStrings["Main"].ConnectionString);
            for (int i = 0; i < 1000; i++)
            {
                var uncommittedEventStream = new UncommittedEventStream(Guid.NewGuid());
                uncommittedEventStream.Append(new UncommittedEvent(Guid.NewGuid(), Guid.NewGuid(), i, i, DateTime.Now, new object(), new Version(1, 0)));                
                eventStore.Store(uncommittedEventStream);
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

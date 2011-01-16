using System;
using System.Threading;
using Ncqrs.EventBus;
using Denormalizer.Properties;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Denormalizer
{
    static class Program
    {
        private static int _processedEvents = 0;
        private static InProcessEventBus _bus = new InProcessEventBus(true);

        static void Main(string[] args)
        {
            // Register all denormalizers in this assembly.
            _bus.RegisterAllHandlersInAssembly(typeof(Program).Assembly);

            var connectionString = Settings.Default.EventStoreConnectionString;
            var browsableEventStore = new MsSqlServerEventStoreElementStore(connectionString);
            var pipeline = Pipeline.Create(new CallbackEventProcessor(Process), browsableEventStore);

            pipeline.Start();

            Console.ReadLine();
            
            pipeline.Stop();
        }

        static void Process(SourcedEvent evnt)
        {
            Thread.Sleep(200);

            Interlocked.Increment(ref _processedEvents);

            Console.WriteLine("Processing event {0} (id {1})", evnt.EventSequence, evnt.EventIdentifier);
            _bus.Publish(evnt);
        }
    }

    public class CallbackEventProcessor : IElementProcessor
    {
        private readonly Action<SourcedEvent> _callback;

        public CallbackEventProcessor(Action<SourcedEvent> callback)
        {
            _callback = callback;
        }

        public void Process(IProcessingElement evnt)
        {
            var typedElement = (SourcedEventProcessingElement) evnt;
            _callback(typedElement.Event);
        }
    }
}

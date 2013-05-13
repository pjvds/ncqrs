using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;

using Ncqrs;
using Ncqrs.CommandService;
using Ncqrs.EventBus;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.SQL;

namespace MyNotes.ApplicationService
{
    static class Program
    {
        public static Func<IBrowsableElementStore> GetBrowsableEventStore = GetBuiltInBrowsableElementStore;

        static void Main(string[] args)
        {
            Console.WindowHeight = 30;
            Console.WindowWidth = 150;

            var bus = new InProcessEventBus(true);
            var eventStore = Program.GetBrowsableEventStore();
            var buffer = new InMemoryBufferedBrowsableElementStore(eventStore, 20 /*magic number found in ThresholdFetchPolicy*/);

            bus.RegisterAllHandlersInAssembly(typeof(MyNotes.Denormalizers.NoteDenormalizer).Assembly);
            BootStrapper.BootUp(buffer);

            var pipeline = Pipeline.Create("Default", new EventBusProcessor(bus), buffer);
            pipeline.Start();

            var commandServiceHost = new ServiceHost(typeof(CommandWebService));
            commandServiceHost.Open();

            Console.ReadLine();

            commandServiceHost.Close();
            pipeline.Stop();
        }

        private static IBrowsableElementStore GetBuiltInBrowsableElementStore()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MyNotes Event Store"].ConnectionString;
            var browsableEventStore = new MsSqlServerEventStoreElementStore(connectionString);

            return browsableEventStore;
        }
    }
}

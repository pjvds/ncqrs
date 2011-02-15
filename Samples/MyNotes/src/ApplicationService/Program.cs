using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using ApplicationService.Properties;
using EventStore.Persistence.SqlPersistence;
using EventStore.Serialization;
using Ncqrs;
using Ncqrs.CommandService;
using Ncqrs.EventBus;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.JOliver;
using Ncqrs.Eventing.Storage.JOliver.SqlPersistence;
using Ncqrs.Eventing.Storage.SQL;

namespace ApplicationService
{
    static class Program
    {
        public static Func<IBrowsableElementStore> GetBrowsableEventStore;
        public static Func<IEventStore> GetEventStore;

        static void Main(string[] args)
        {
            GetBrowsableEventStore = GetBuiltInBrowsableElementStore;
            GetEventStore = GetBuiltInEventStore;

            
            var bus = new InProcessEventBus(true);
            bus.RegisterAllHandlersInAssembly(typeof(Program).Assembly);
            var buffer = new InMemoryBufferedBrowsableElementStore(GetBrowsableEventStore(), 20 /*magic number found in ThresholedFetchPolicy*/);
            var pipeline = Pipeline.Create("Default", new EventBusProcessor(bus), buffer);

            BootStrapper.BootUp(buffer, GetEventStore());
            var commandServiceHost = new ServiceHost(typeof(CommandWebService));

            commandServiceHost.Open();
            pipeline.Start();

            Console.ReadLine();

            pipeline.Stop();
            commandServiceHost.Close();
        }

        private static IEventStore GetJoesEventStore()
        {
            var factory = new AbsoluteOrderingSqlPersistenceFactory("EventStore", new BinarySerializer());
            var streamPersister = factory.Build();
            streamPersister.Initialize();
            var store = new JoesEventStoreAdapter(streamPersister);
            return store;
        }

        private static IEventStore GetBuiltInEventStore()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["EventStore"].ConnectionString;
            return new MsSqlServerEventStore(connectionString);
        }

        private static IBrowsableElementStore GetJoesBrowsableElementStore()
        {
            var factory = new AbsoluteOrderingSqlPersistenceFactory("EventStore", new BinarySerializer());
            var store = factory.Build();
            store.Initialize();
            return new JoesBrowsableEventStore(store);
        }

        private static IBrowsableElementStore GetBuiltInBrowsableElementStore()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["EventStore"].ConnectionString;
            var browsableEventStore = new MsSqlServerEventStoreElementStore(connectionString);
            return browsableEventStore;
        }
    }
}

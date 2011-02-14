using System;
using System.Collections.Generic;
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
using Ncqrs.Eventing.Storage.JOliver;
using Ncqrs.Eventing.Storage.JOliver.SqlPersistence;

namespace ApplicationService
{
    static class Program
    {
        static void Main(string[] args)
        {
            var factory = new AbsoluteOrderingSqlPersistenceFactory("EventStore", new BinarySerializer());
            var store = factory.Build();
            store.Initialize();

            var bus = new InProcessEventBus(true);
            bus.RegisterAllHandlersInAssembly(typeof(Program).Assembly);
            var browsableEventStore = new JoesBrowsableEventStore(store);
            var buffer = new InMemoryBufferedBrowsableElementStore(browsableEventStore, 20 /*magic number faund in ThresholedFetchPolicy*/);
            var pipeline = Pipeline.Create("Default", new EventBusProcessor(bus), buffer);

            BootStrapper.BootUp(buffer);
            var commandServiceHost = new ServiceHost(typeof(CommandWebService));

            commandServiceHost.Open();
            pipeline.Start();

            Console.ReadLine();

            pipeline.Stop();
            commandServiceHost.Close();
        }
    }
}

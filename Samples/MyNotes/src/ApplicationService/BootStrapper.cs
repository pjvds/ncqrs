using ApplicationService.Properties;
using Commands;
using EventStore.Persistence.SqlPersistence;
using EventStore.Serialization;
using Ncqrs;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.CommandService.Infrastructure;
using Ncqrs.EventBus;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.JOliver;
using Ncqrs.Eventing.Storage.SQL;

namespace ApplicationService
{
    public static class BootStrapper
    {
        public static void BootUp(InMemoryBufferedBrowsableElementStore buffer)
        {
            var config = new Ncqrs.Config.StructureMap.StructureMapConfiguration(cfg =>
            {
                cfg.For<ICommandService>().Use(InitializeCommandService);
                cfg.For<IEventBus>().Use(x => InitializeEventBus(buffer));
                cfg.For<IEventStore>().Singleton().Use(InitializeEventStore);
                cfg.For<IKnownCommandsEnumerator>().Use(new AllCommandsInAppDomainEnumerator());
            });

            NcqrsEnvironment.Configure(config);
        }

        private static ICommandService InitializeCommandService()
        {
            var commandAssembly = typeof (CreateNewNote).Assembly;

            var service = new CommandService();
            service.RegisterExecutorsInAssembly(commandAssembly);
            service.AddInterceptor(new ThrowOnExceptionInterceptor());

            return service;
        }

        private static IEventStore InitializeEventStore()
        {
            var factory = new SqlPersistenceFactory("EventStore", new BinarySerializer());
            var streamPersister = factory.Build();
            streamPersister.Initialize();
            var store = new JoesEventStoreAdapter(streamPersister);
            return store;
        }

        private static IEventBus InitializeEventBus(InMemoryBufferedBrowsableElementStore buffer)
        {
            var bus = new InProcessEventBus();

            bus.RegisterHandler(new InMemoryBufferedEventHandler(buffer));

            return bus;
        }
    }
}
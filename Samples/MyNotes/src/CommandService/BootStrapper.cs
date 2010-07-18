using System;
using CommandService.Properties;
using Ncqrs;
using Ncqrs.Commanding.CommandExecution.Mapping;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Commanding.ServiceModel;
using Commands;
using Ncqrs.Config.StructureMap;
using Ncqrs.Eventing.ServiceModel.Bus;
using ReadModel.Denormalizers;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.SQL;

namespace CommandService
{
    public static class BootStrapper
    {
        public static void BootUp()
        {
            var config = new StructureMapConfiguration(cfg =>
            {
                cfg.For<ICommandService>().Use(InitializeCommandService);
                cfg.For<IEventBus>().Use(InitializeEventBus);
                cfg.For<IEventStore>().Use(InitializeEventStore);
            });

            NcqrsEnvironment.Configure(config);
        }

        private static ICommandService InitializeCommandService()
        {
            var commandAssembly = typeof (CreateNewNote).Assembly;

            var service = new Ncqrs.Commanding.ServiceModel.CommandService();
            service.RegisterExecutorsInAssembly(commandAssembly);

            return service;
        }

        private static IEventStore InitializeEventStore()
        {
            var store = new MsSqlServerEventStore(Settings.Default.SqlEventStoreConnectionString);
            return store;
        }

        private static IEventBus InitializeEventBus()
        {
            var bus = new InProcessEventBus();
            bus.RegisterAllHandlersInAssembly(typeof(NoteItemDenormalizer).Assembly);

            return bus;
        }
    }
}
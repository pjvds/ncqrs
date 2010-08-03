using System;
using Commands;
using CommandService.Properties;
using Events;
using Ncqrs;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Config.StructureMap;
using Ncqrs.Domain;
using Ncqrs.Domain.Storage;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.NoDB;
using Ncqrs.Eventing.Storage.SQL;
using ReadModel.Denormalizers;

namespace Tests
{
    public static class BootStrapper
    {
        public static void BootUp(PerformanceTests handler)
        {
            var config = new StructureMapConfiguration(cfg =>
            {
                cfg.For<ICommandService>().Use(InitializeCommandService);
                cfg.For<IEventBus>().Use(InitializeEventBus(handler));
                cfg.For<IEventStore>().Use(InitializeEventStore);
                cfg.For<ISnapshotStore>().Use(() => new NoDBSnapshotStore("TestStore"));
                cfg.For<IUnitOfWorkFactory>().Use(() => new SnapshottingUnitOfWorkFactory());
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
            //return new MsSqlServerEventStore(@"Data Source=.\sqlexpress;Initial Catalog=MyNotesEventStore;Integrated Security=SSPI;");
            return new NoDBEventStore("TestStore");
        }

        private static IEventBus InitializeEventBus(IEventHandler<NewNoteAdded> handler)
        {
            var bus = new InProcessEventBus();
            bus.RegisterHandler(handler);

            return bus;
        }
    }

    public class SnapshottingUnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWorkContext CreateUnitOfWork()
        {
            if (UnitOfWork.Current != null) throw new InvalidOperationException("There is already a unit of work created for this context.");

            var store = NcqrsEnvironment.Get<IEventStore>();
            var bus = NcqrsEnvironment.Get<IEventBus>();
            var snapshotstore = NcqrsEnvironment.Get<ISnapshotStore>();

            var repository = new DomainRepository(store, bus, snapshotstore);
            return new UnitOfWork(repository);
        }

    }
}
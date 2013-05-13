using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

using Castle.MicroKernel.Registration;
using Castle.Windsor;

using Ncqrs;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.CommandService.Infrastructure;
using Ncqrs.Config.Windsor;
using Ncqrs.EventBus;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.SQL;

using MyNotes.Commands;
using MyNotes.Domain;

namespace MyNotes.ApplicationService
{
    internal static class BootStrapper
    {
        public static void BootUp(InMemoryBufferedBrowsableElementStore buffer)
        {
            ConfigureNcqrsEnvironment(buffer);
        }

        private static void ConfigureNcqrsEnvironment(InMemoryBufferedBrowsableElementStore buffer)
        {
            var eventStoreConnectionString = ConfigurationManager.ConnectionStrings["MyNotes Event Store"].ConnectionString;
            var eventStore = new MsSqlServerEventStore(eventStoreConnectionString);

            Assembly domainAssembly = Assembly.LoadFrom("MyNotes.Domain.dll");

            IWindsorContainer container = new WindsorContainer();
            container.AddFacility("ncqrs.ds", new DynamicSnapshotFacility(domainAssembly));
            container.Register(
                Component.For<ISnapshottingPolicy>().ImplementedBy<SimpleSnapshottingPolicy>(),
                Component.For<ICommandService>().Instance(InitializeCommandService()),
                Component.For<IEventBus>().Instance(InitializeEventBus(buffer)),
                Component.For<IEventStore>().Forward<ISnapshotStore>().Instance(eventStore),
                Component.For<IKnownCommandsEnumerator>().Instance(new AllCommandsInAppDomainEnumerator()),
                Component.For<Note>().AsSnapshotable());

            WindsorConfiguration config = new WindsorConfiguration(container);

            NcqrsEnvironment.Configure(config);
        }

        private static ICommandService InitializeCommandService()
        {
            var commandAssembly = typeof(CreateNewNote).Assembly;
            var service = new CommandService();

            service.RegisterExecutorsInAssembly(commandAssembly);
            service.AddInterceptor(new ThrowOnExceptionInterceptor());

            return service;
        }

        private static IEventBus InitializeEventBus(InMemoryBufferedBrowsableElementStore buffer)
        {
            var bus = new InProcessEventBus();

            bus.RegisterHandler(new InMemoryBufferedEventHandler(buffer));

            return bus;
        }
    }
}

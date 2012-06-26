using Commands;
using Ncqrs;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.CommandService.Infrastructure;
using Ncqrs.EventBus;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.AWS;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Ncqrs.Config.Windsor;
using Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot;
using System.Reflection;

namespace ApplicationService
{
    using MyProject.Domain;

    public static class BootStrapper
    {
        public static void BootUp()
        {
            var asd = new SimpleDBStore();
            var dsa = new SimpleDBStore();

            Assembly asm = Assembly.LoadFrom("Domain.dll");
            IWindsorContainer container = new WindsorContainer();
            container.AddFacility("ncqrs.ds", new DynamicSnapshotFacility(asm));
            container.Register(
                Component.For<ISnapshottingPolicy>().ImplementedBy<SimpleSnapshottingPolicy>(),
                Component.For<ICommandService>().Instance(InitializeCommandService()),
                Component.For<IEventBus>().Instance(InitializeEventBus()),
                Component.For<IEventStore>().Forward<ISnapshotStore>().Instance(dsa),
                Component.For<IKnownCommandsEnumerator>().Instance(new AllCommandsInAppDomainEnumerator()),
                Component.For<Note>().AsSnapshotable()
                );


            WindsorConfiguration config = new WindsorConfiguration(container);

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

        private static IEventBus InitializeEventBus()
        {
            var bus = new InProcessEventBus();
            
            return bus;
        }
    }
}
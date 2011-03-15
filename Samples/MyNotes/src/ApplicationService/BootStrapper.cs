using System.Configuration;
using Commands;
using Ncqrs;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.CommandService.Infrastructure;
using Ncqrs.EventBus;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.SQL;
using StructureMap;

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
                InitializeBuiltInSqlEventStrore(cfg);
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

        private static IEventBus InitializeEventBus(InMemoryBufferedBrowsableElementStore buffer)
        {
            var bus = new InProcessEventBus();

            bus.RegisterHandler(new InMemoryBufferedEventHandler(buffer));

            return bus;
        }

        private static void InitializeBuiltInSqlEventStrore(IInitializationExpression cfg)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["EventStore"].ConnectionString;
            cfg.For<IEventStore>().Use(new MsSqlServerEventStore(connectionString));
        }
    }
}
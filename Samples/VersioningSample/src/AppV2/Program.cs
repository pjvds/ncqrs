using System;
using System.Configuration;
using AwesomeAppRefactored.Commands;
using Ncqrs;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.SQL;

namespace AwesomeAppRefactored
{
    class Program
    {
        static void Main(string[] args)
        {
            NcqrsEnvironment.SetDefault(InitializeEventStore());
            NcqrsEnvironment.SetDefault(InitializeCommandService());

            var commandService = NcqrsEnvironment.Get<ICommandService>();
            var id = new Guid("AE6920ED-381A-467D-8DB2-EE91E851F431");

            commandService.Execute(new ChangeNameCommand(id, "Jane", "Doe"));

            Console.WriteLine("If you see this message and no exception occurred, it had probably worked. Now you can run AppV3.");
            Console.ReadLine();
        }

        private static IEventStore InitializeEventStore()
        {
            var typeResolver = new AttributeEventTypeResolver();
            typeResolver.AddAllEventsInAssembly(typeof(Program).Assembly);
            
            var converter = new PropertyBagConverter();
            converter.TypeResolver = typeResolver;

            var eventStore = new MsSqlServerEventStore(ConfigurationManager.ConnectionStrings["EventStore"].ConnectionString, converter);
            return eventStore;
        }

        private static ICommandService InitializeCommandService()
        {
            var service = new CommandService();
            service.RegisterExecutor(new AttributeMappedCommandExecutor<CreatePersonCommand>());
            service.RegisterExecutor(new AttributeMappedCommandExecutor<ChangeNameCommand>());
            return service;
        }
    }
}

using System;
using System.Configuration;
using AwesomeAppRefactored.Commands;
using Ncqrs;
using Ncqrs.Commanding.CommandExecution.Mapping;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.Serialization;
using Ncqrs.Eventing.Storage.SQL;
using AwesomeAppRefactored.Events;

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
            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
        }

        private static IEventStore InitializeEventStore()
        {
            var typeResolver = new AttributeEventTypeResolver();
            typeResolver.AddEvent(typeof(NameChangedEvent));
            typeResolver.AddEvent(typeof(PersonCreatedEvent)); 
            
            var eventStore = new MsSqlServerEventStore(ConfigurationManager.ConnectionStrings["EventStore"].ConnectionString, typeResolver, null);
            return eventStore;
        }

        private static ICommandService InitializeCommandService()
        {
            var mapper = new AttributeBasedCommandMapper();
            var service = new CommandService();
            service.RegisterExecutor(typeof(CreatePersonCommand), new UoWMappedCommandExecutor(mapper));
            service.RegisterExecutor(typeof(ChangeNameCommand), new UoWMappedCommandExecutor(mapper));
            return service;
        }
    }
}

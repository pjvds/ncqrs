using System;
using System.Configuration;
using AwesomeAppRefactored.Commands;
using AwesomeAppRefactored.Events;
using Ncqrs;
using Ncqrs.Commanding.CommandExecution.Mapping;
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

            commandService.Execute(new ChangeNameCommand(id, "Jane Smith Doe"));

            Console.WriteLine("If you see this message and no exception occurred, it had probably worked. You are all clear now!");
            Console.ReadKey(true);
        }

        private static IEventStore InitializeEventStore()
        {
            var typeResolver = new AttributeEventTypeResolver();
            typeResolver.AddAllEventsInAssembly(typeof(Program).Assembly);
            
            var converter = new PropertyBagConverter();
            converter.TypeResolver = typeResolver;

            converter.AddPostConversion(typeof(NameChangedEvent), new NameChangedEventPostConverter());
            converter.AddPostConversion(typeof(PersonCreatedEvent), new PersonCreatedEventPostConverter());

            var eventStore = new MsSqlServerEventStore(ConfigurationManager.ConnectionStrings["EventStore"].ConnectionString, converter);
            return eventStore;
        }

        private static ICommandService InitializeCommandService()
        {
            var service = new CommandService();
            service.RegisterExecutor(new MappedCommandExecutor<CreatePersonCommand>());
            service.RegisterExecutor(new MappedCommandExecutor<ChangeNameCommand>());
            return service;
        }
    }
}

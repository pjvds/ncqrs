using System;
using AwesomeAppRefactored.Commands;
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
        }

        private static IEventStore InitializeEventStore()
        {
            var eventStore = new SimpleMicrosoftSqlServerEventStore("Data Source=.; Initial Catalog=VersioningEventStore; Integrated Security=SSPI;");
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

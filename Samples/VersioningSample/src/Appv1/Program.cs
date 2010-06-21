using System;
using AwesomeApp.Commands;
using Ncqrs;
using Ncqrs.Commanding.CommandExecution.Mapping;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.SQL;

namespace AwesomeApp
{
    class Program
    {
        static void Main(string[] args)
        {
            NcqrsEnvironment.SetDefault(InitializeEventStore());
            NcqrsEnvironment.SetDefault(InitializeCommandService());

            var commandService = NcqrsEnvironment.Get<ICommandService>();
            var id = new Guid("AE6920ED-381A-467D-8DB2-EE91E851F431");

            commandService.Execute(new CreatePersonCommand(id, "John", "Smith"));
            commandService.Execute(new ChangeNameCommand(id, "Jane", "Smith"));

            Console.WriteLine("If you see this message and no exception occurred, it had probably worked. Now you can run AppV2.");
            Console.ReadKey(true);
        }

        private static IEventStore InitializeEventStore()
        {
            var eventStore = new MsSqlServerEventStore("Data Source=.\\sqlexpress; Initial Catalog=VersioningEventStore; Integrated Security=SSPI;");
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

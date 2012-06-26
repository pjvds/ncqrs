using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Ncqrs.Eventing.Storage.AWS.Tests.Env
{
    public static class Startup
    {
        public static void Start()
        {
            NcqrsEnvironment.SetDefault<IEventStore>(new SimpleDBStore("MainTest"));
            CommandService c = new CommandService();

            c.RegisterExecutorsInAssembly(typeof(CreateNoteCommand).Assembly);

            NcqrsEnvironment.SetDefault<ICommandService>(c);
        }
    }
}

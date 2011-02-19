using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Eventing.Storage.SQL;

namespace Ncqrs.Eventing.Storage.WindowsAzure.Tests.Env
{
    public static class Startup
    {
        public static void Start()
        {
            NcqrsEnvironment.SetDefault<IEventStore>(new TableOnlyStore("MainTest"));
            //NcqrsEnvironment.SetDefault<IEventStore>(new MsSqlServerEventStore(@"Server=.\SQLExpress;Initial Catalog=MyNotesEventStore;Integrated Security=SSPI"));
            CommandService c = new CommandService();
            
            c.RegisterExecutorsInAssembly(typeof(CreateNoteCommand).Assembly);


            NcqrsEnvironment.SetDefault<ICommandService>(c);
        }
    }
}

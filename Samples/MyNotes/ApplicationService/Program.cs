using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using ApplicationService.Properties;
using Ncqrs;
using Ncqrs.CommandService;
using Ncqrs.EventBus;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Sourcing;

namespace ApplicationService
{
    static class Program
    {
        static void Main(string[] args)
        {
            var bus = new InProcessEventBus(true);
            bus.RegisterAllHandlersInAssembly(typeof(Program).Assembly);
            var browsableEventStore = new MsSqlServerEventStoreElementStore(Settings.Default.EventStoreConnectionString);
            var buffer = new InMemoryBufferedBrowsableElementStore(browsableEventStore, 20 /*magic number faund in ThresholedFetchPolicy*/);
            //var pipeline = Pipeline.Create("FirstPipeline", new EventBusProcessor(bus), buffer);

            BootStrapper.BootUp(buffer);
            var commandServiceHost = new ServiceHost(typeof(CommandWebService));

            commandServiceHost.Open();
            //pipeline.Start();

            Console.ReadLine();

            //pipeline.Stop();
            commandServiceHost.Close();
        }
    }
}

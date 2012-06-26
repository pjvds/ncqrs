using System;
using System.Configuration;
using System.ServiceModel;
using Ncqrs.CommandService;
using Ncqrs.EventBus;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace ApplicationService
{
    static class Program
    {
        public static Func<IBrowsableElementStore> GetBrowsableEventStore;

        static void Main(string[] args)
        {
            var bus = new InProcessEventBus(true);
            bus.RegisterAllHandlersInAssembly(typeof(Program).Assembly);
            
            BootStrapper.BootUp();
            var commandServiceHost = new ServiceHost(typeof(CommandWebService));

            commandServiceHost.Open();
            

            Console.ReadLine();

            commandServiceHost.Close();
        }
    }
}

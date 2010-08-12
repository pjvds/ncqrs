using System;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Ncqrs.EventBus.IntegrationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Pipeline(
                new ConsoleEventProcessor(), 
                new InMemoryPipelineBackupQueue(), 
                new MsSqlServerPipelineStateStore(ConfigurationManager.ConnectionStrings["Main"].ConnectionString), 
                new FakeEventStore(), 
                new ThresholdedEventFetchPolicy(10, 20));
            p.Start();

            Console.WriteLine("Press any key");

            Console.ReadLine();
        }
    }
}

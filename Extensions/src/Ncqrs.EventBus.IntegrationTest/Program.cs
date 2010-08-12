using System;
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
                new InMemoryPipelineStateStore(), 
                new FakeEventStore(), 
                new ThresholdedEventFetchPolicy(10, 50));
            p.Start();

            Console.WriteLine("Press any key");

            Console.ReadLine();
        }
    }
}

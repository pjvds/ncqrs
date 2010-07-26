using System;
using Commands;
using Events;
using Ncqrs.Commanding;
using Ncqrs.NServiceBus;
using NServiceBus;

namespace Client
{
    public class ClientEndpoint : IWantToRunAtStartup
    {
        public static Guid AggregateId;

        public IBus Bus { get; set; }

        public void Run()
        {
            Console.WriteLine("Press 'Enter' to send a message to create a new Aggregate.To exit, Ctrl + C");
            Console.ReadLine();

            Bus.Send("ServerQueue", new CommandMessage { Payload = new CreateSomeObjectCommand () });

            string line;
            while ((line = Console.ReadLine()) != null)
            {
                ICommand payload = new DoSomethingCommand { Value = line, ObjectId = AggregateId };
                var command = new CommandMessage
                                 {
                                     Payload = payload
                                 };
                Bus.Send("ServerQueue", command);
            }
        }

        public void Stop()
        {

        }
    }
}

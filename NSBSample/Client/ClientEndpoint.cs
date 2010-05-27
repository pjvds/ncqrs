using System;
using Commands;
using Ncqrs.NServiceBus;
using NServiceBus;

namespace Client
{
   public class ClientEndpoint : IWantToRunAtStartup
   {
      public IBus Bus { get; set; }

      public void Run()
      {
         Console.WriteLine("This will send commands containing text you write.");
         Console.WriteLine("Press 'Enter' to send a message.To exit, Ctrl + C");

         string line;
         while ((line = Console.ReadLine()) != null)
         {
            var command = new CommandMessage
                             {
                                Payload = new DoSomethingCommand
                                             {
                                                Value = line
                                             }
                             };

            Bus.Send("ServerQueue",command);
         }
      }

      public void Stop()
      {

      }      
   }
}

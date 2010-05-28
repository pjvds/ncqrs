using System;
using Ncqrs.Commanding.ServiceModel;
using NServiceBus;

namespace Ncqrs.NServiceBus
{
   public class NcqrsMessageHandler : IHandleMessages<CommandMessage>
   {      
      public ICommandService CommandService { get; set;}

      public void Handle(CommandMessage message)
      {
         CommandService.Execute(message.Payload);
      }
   }
}
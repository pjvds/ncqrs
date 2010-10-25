using System;
using Ncqrs.Commanding;
using NServiceBus;

namespace Ncqrs.NServiceBus
{
   [Serializable]
   public class CommandMessage : IMessage, Ncqrs.NServiceBus.ICommandMessage
   {
      public ICommand Payload { get; set; }
   }
}
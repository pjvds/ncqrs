using System;
using Ncqrs.Commanding;
using NServiceBus;

namespace Ncqrs.NServiceBus
{
   [Serializable]
   public class CommandMessage : IMessage
   {
      public ICommand Payload { get; set; }
   }
}
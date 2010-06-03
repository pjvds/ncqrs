using System;
using Ncqrs.Domain;

namespace Ncqrs.Messaging
{
   [Serializable]
   public class MessageReceivedEvent : DomainEvent
   {      
      public DateTime ReceiveTime { get; set; }
      public IMessage Message { get; set; }

      public MessageReceivedEvent()
      {         
      }

      public MessageReceivedEvent(DateTime receiveTime, IMessage message)
      {
         ReceiveTime = receiveTime;
         Message = message;
      }
   }
}
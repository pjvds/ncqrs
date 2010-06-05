using System;
using Ncqrs.Domain;

namespace Ncqrs.Messaging
{
   [Serializable]
   public class MessageReceivedEvent : DomainEvent
   {      
      public DateTime ReceiveTime { get; set; }
      public IncomingMessage Message { get; set; }

      public MessageReceivedEvent()
      {         
      }

      public MessageReceivedEvent(DateTime receiveTime, IncomingMessage message)
      {
         ReceiveTime = receiveTime;
         Message = message;
      }
   }
}
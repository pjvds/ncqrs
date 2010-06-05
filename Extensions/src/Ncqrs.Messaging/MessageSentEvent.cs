using System;
using Ncqrs.Domain;

namespace Ncqrs.Messaging
{
   [Serializable]
   public class MessageSentEvent : DomainEvent
   {
      public DateTime SentTime { get; set; }
      public OutgoingMessage Message { get; set; }

      public MessageSentEvent()
      {         
      }

      public MessageSentEvent(DateTime sentTime, OutgoingMessage message)
      {
         SentTime = sentTime;
         Message = message;
      }
   }
}
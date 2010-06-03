using System;
using Ncqrs.Domain;

namespace Ncqrs.Messaging
{
   [Serializable]
   public class MessageSentEvent : DomainEvent
   {
      public DateTime SentTime { get; set; }
      public IMessage Message { get; set; }

      public MessageSentEvent()
      {         
      }

      public MessageSentEvent(DateTime sentTime, IMessage message)
      {
         SentTime = sentTime;
         Message = message;
      }
   }
}
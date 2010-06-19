using System;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Messaging
{
   [Serializable]
   public class MessageSentEvent : SourcedEvent
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
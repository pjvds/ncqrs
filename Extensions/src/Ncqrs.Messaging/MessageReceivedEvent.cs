using System;

namespace Ncqrs.Messaging
{
   [Serializable]
   public class MessageReceivedEvent
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
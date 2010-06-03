using System;

namespace Ncqrs.Messaging
{
   public abstract class MessageBase : IMessage
   {
      public Guid MessageId { get; set; }
      public Guid? RelatedMessageId { get; set; }
      public string ReceiverId { get; set; }
      public string SenderId { get; set; }
      public MessageProcessingRequirements ProcessingRequirements { get; set; }
   }
}
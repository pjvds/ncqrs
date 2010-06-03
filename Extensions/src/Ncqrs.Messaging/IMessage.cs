using System;

namespace Ncqrs.Messaging
{
   /// <summary>
   /// A message. Messages can be passed between aggregates.
   /// </summary>
   public interface IMessage
   {      
      /// <summary>
      /// Gets the unique identifier of this message.
      /// </summary>
      Guid MessageId { get; set; }
      /// <summary>
      /// Gets the identifier of message related to this one (for response message that would
      /// be the id of request message).
      /// </summary>
      Guid? RelatedMessageId { get; set; }
      /// <summary>
      /// Gets the identifier of aggregate which is receiver of this message.
      /// </summary>
      string ReceiverId { get; set; }      
      /// <summary>
      /// Gets the identifier of aggregate which is sender of this message.
      /// </summary>
      string SenderId { get; set; }
      /// <summary>
      /// Gets the processing requirements for this message.
      /// </summary>
      MessageProcessingRequirements ProcessingRequirements { get; set; }      
   }
}
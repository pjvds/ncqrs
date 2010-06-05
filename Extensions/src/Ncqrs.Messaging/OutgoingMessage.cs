using System;

namespace Ncqrs.Messaging
{
    public class OutgoingMessage
    {
        public object Payload { get; set; }
        /// <summary>
        /// Gets the unique identifier of this message.
        /// </summary>
        public Guid MessageId { get; set; }
        /// <summary>
        /// Gets the identifier of message related to this one (for response message that would
        /// be the id of request message).
        /// </summary>
        public Guid? RelatedMessageId { get; set; }
        /// <summary>
        /// Gets the identifier of aggregate which is sender of this message.
        /// </summary>
        public Guid SenderId { get; set; }
        public Type SenderType { get; set; }
        /// <summary>
        /// Gets the identifier of aggregate which is receiver of this message.
        /// </summary>
        public string ReceiverId { get; set; }        
        /// <summary>
        /// Gets the processing requirements for this message.
        /// </summary>
        public MessageProcessingRequirements ProcessingRequirements { get; set; }
    }
}
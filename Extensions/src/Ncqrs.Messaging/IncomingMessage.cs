using System;

namespace Ncqrs.Messaging
{
    public class IncomingMessage
    {
        public object Payload { get; set; }
        public Guid MessageId { get; set; }
        public Guid? RelatedMessageId { get; set; }
        public Guid ReceiverId { get; set; }
        public Type ReceiverType { get; set; }
        public string SenderId { get; set; }
        public MessageProcessingRequirements ProcessingRequirements { get; set; }
    }
}
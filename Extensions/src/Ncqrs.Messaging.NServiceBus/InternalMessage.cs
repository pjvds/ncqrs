using System;
using NServiceBus;

namespace Ncqrs.Messaging.NServiceBus
{
    [Serializable]
    public class InternalMessage : IMessage
    {
        public object Payload { get; set; }
        public Guid MessageId { get; set; }
        public Guid? RelatedMessageId { get; set; }
        public Guid ReceiverId { get; set; }
        public Type ReceiverType { get; set; }
        public Guid SenderId { get; set; }
        public Type SenderType { get; set; }
        public MessageProcessingRequirements ProcessingRequirements { get; set; }
    }
}
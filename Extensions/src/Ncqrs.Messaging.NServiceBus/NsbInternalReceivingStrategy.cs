namespace Ncqrs.Messaging.NServiceBus
{
    public class NsbInternalReceivingStrategy : IReceivingStrategy
    {
        private static readonly IAddressing _addressing = new UrlAddressing();

        public IncomingMessage Receive(object message)
        {
            var typedMessage = (InternalMessage)message;
            return new IncomingMessage
                       {
                           MessageId = typedMessage.MessageId,
                           Payload = typedMessage.Payload,
                           ProcessingRequirements = typedMessage.ProcessingRequirements,
                           ReceiverId = typedMessage.ReceiverId,
                           ReceiverType = typedMessage.ReceiverType,
                           RelatedMessageId = typedMessage.RelatedMessageId,
                           SenderId = _addressing.EncodeAddress(new Destination(typedMessage.SenderType, typedMessage.SenderId))
                       };
        }
    }
}
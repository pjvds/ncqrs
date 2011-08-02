using System;
using System.Text.RegularExpressions;

namespace Ncqrs.Messaging
{
    public class LocalReceivingStrategy : IReceivingStrategy
    {
        private static readonly IAddressing _addressing = new UrlAddressing();

        public IncomingMessage Receive(object message)
        {
            var typedMessage = (OutgoingMessage) message;
            var destination = _addressing.DecodeAddress(typedMessage.ReceiverId);
            return new IncomingMessage
                       {
                           MessageId = typedMessage.MessageId,
                           Payload = typedMessage.Payload,
                           ProcessingRequirements = typedMessage.ProcessingRequirements,
                           ReceiverId = destination.Id,
                           ReceiverType = destination.Type,
                           RelatedMessageId = typedMessage.RelatedMessageId,
                           SenderId = _addressing.EncodeAddress(new Destination(typedMessage.SenderType, typedMessage.SenderId))
                       };
        }
    }
}
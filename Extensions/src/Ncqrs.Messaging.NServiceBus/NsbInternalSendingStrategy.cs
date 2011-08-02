using System;
using NServiceBus;

namespace Ncqrs.Messaging.NServiceBus
{
    public class NsbInternalSendingStrategy : ISendingStrategy
    {
        private static readonly IAddressing _addressing = new UrlAddressing();

        public void Send(OutgoingMessage message)
        {
            var destination = _addressing.DecodeAddress(message.ReceiverId);
            Bus.SendLocal(new InternalMessage
                              {
                                  MessageId = message.MessageId,
                                  Payload = message.Payload,
                                  ProcessingRequirements = message.ProcessingRequirements,
                                  ReceiverId = destination.Id,
                                  ReceiverType = destination.Type,
                                  RelatedMessageId = message.RelatedMessageId,
                                  SenderId = message.SenderId,
                                  SenderType = message.SenderType
                              });
        }

        private static IBus Bus
        {
            get { return NcqrsEnvironment.Get<IBus>(); }
        }
    }
}
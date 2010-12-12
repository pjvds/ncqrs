using System;
using Ncqrs.Domain;

namespace Ncqrs.Messaging
{
    public class MessagingAggregateRoot : AggregateRootMappedByConvention, IMessagingAggregateRoot
    {
        private readonly MessagingContext _messagingContext = new MessagingContext();

        public MessagingAggregateRoot() : base()
        {            
        }

        public MessagingAggregateRoot(Guid id) : base(id)
        {            
        }

        public MessagingContext MessagingContext
        {
            get { return _messagingContext; }
        }

        void IMessagingAggregateRoot.ProcessMessage(IncomingMessage message)
        {
            if (MessagingContext.WasAlreadyProcessed(message))
            {
                return;
            }
            var processor = CreateProcessor(message);
            try
            {
                MessagingContext.OnBeginMessageProcessing(message);
                processor.Process(this, message);
                ApplyEvent(new MessageReceivedEvent(DateTime.Now, message));
            }
            finally
            {
                MessagingContext.OnEndMessageProcessing();
            }
        }

        protected void Reply(object payload)
        {
            var message = new OutgoingMessage
                              {
                                  Payload = payload,
                                  SenderId = EventSourceId,
                                  SenderType = GetType(),
                                  MessageId = NcqrsEnvironment.Get<IUniqueIdentifierGenerator>().GenerateNewId(),
                                  ReceiverId = MessagingContext.MessageBeingProcessed.SenderId,
                                  RelatedMessageId = MessagingContext.MessageBeingProcessed.MessageId
                              };
            ApplyEvent(new MessageSentEvent(DateTime.Now, message));
        }

        protected ISetMessageDestination To()
        {
            return new FluentMessageSender(EventSourceId, x => ApplyEvent(new MessageSentEvent(DateTime.Now, x)));
        }

        protected void OnMessageReceivedEvent(MessageReceivedEvent @event)
        {
            MessagingContext.OnReceived(@event);
        }

        protected void OnMessageSentEvent(MessageSentEvent @event)
        {
            MessagingContext.OnSent(@event);
        }

        private interface IMessageProcessor
        {
            void Process(object aggregateRoot, IncomingMessage message);
        }

        private static IMessageProcessor CreateProcessor(IncomingMessage message)
        {
            Type processorType = typeof(MessageProcessor<>).MakeGenericType(message.Payload.GetType());
            return (IMessageProcessor)Activator.CreateInstance(processorType);
        }

        private class MessageProcessor<T> : IMessageProcessor           
        {
            public void Process(object aggregateRoot, IncomingMessage message)
            {
                var handler = aggregateRoot as IMessageHandler<T>;
                if (handler == null)
                {
                    throw new MessageMappingException(message.GetType(), aggregateRoot.GetType());
                }
                handler.Handle((T)message.Payload);
            }
        }

        private class FluentMessageSender : ISetMessageRequirements, ISetMessageDestination
        {
            private readonly Guid _senderId;
            private readonly OutgoingMessage _message;
            private readonly Action<OutgoingMessage> _sendAction;

            public FluentMessageSender(Guid senderId, Action<OutgoingMessage> sendAction)
            {
                _message = new OutgoingMessage();
                _senderId = senderId;
                _sendAction = sendAction;
            }

            public void Send(object payload)
            {
                _message.Payload = payload;
                _message.SenderId = _senderId;
                _message.SenderType = GetType();
                _message.MessageId = NcqrsEnvironment.Get<IUniqueIdentifierGenerator>().GenerateNewId();
                _sendAction(_message);
            }

            public ISendMessages Ensuring(MessageProcessingRequirements requirements)
            {
                _message.ProcessingRequirements = requirements;
                return this;
            }

            public ISetMessageRequirements Endpoint(string receiverId)
            {
                _message.ReceiverId = receiverId;
                return this;
            }
        }
    }
}
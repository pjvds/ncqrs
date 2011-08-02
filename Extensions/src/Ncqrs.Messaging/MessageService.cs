using System;
using System.Collections.Generic;
using System.Linq;
using Ncqrs.Domain;

namespace Ncqrs.Messaging
{
    public class MessageService : IMessageService
    {
        private readonly List<ConditionalReceivingStrategy> _receiverResolutionStrategies;

        public MessageService()
        {
            _receiverResolutionStrategies = new List<ConditionalReceivingStrategy>();
        }

        public void UseReceivingStrategy(ConditionalReceivingStrategy receivingStrategy)
        {
            _receiverResolutionStrategies.Add(receivingStrategy);
        }

        public void Process(object message)
        {
            var incomingMessage = ReceiveMessage(message);
            using (var work = NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork(incomingMessage.MessageId))
            {
                var targetAggregateRoot = GetReceiver(work, incomingMessage);
                targetAggregateRoot.ProcessMessage(incomingMessage);
                work.Accept();
            }
        }

        private static IMessagingAggregateRoot GetReceiver(IUnitOfWorkContext work, IncomingMessage message)
        {                        
            var existingReceiver = (IMessagingAggregateRoot)work.GetById(message.ReceiverType, message.ReceiverId, null);
            CheckProcessingRequirements(message, existingReceiver);

            return existingReceiver ?? CreateNewAggregateInstance(message.ReceiverType, message.ReceiverId);
        }

        private IncomingMessage ReceiveMessage(object message)
        {
            return _receiverResolutionStrategies
                .Select(x => x.Receive(message))
                .FirstOrDefault(x => x != null);            
        }

        private static void CheckProcessingRequirements(IncomingMessage message, object existingReceiver)
        {
            if (ExpectedNoneButFoundOne(message, existingReceiver) ||
                ExpectedOneButFoundNone(message, existingReceiver))
            {
                throw new MessageProcessingRequirementsViolationException(message);
            }
        }

        private static bool ExpectedOneButFoundNone(IncomingMessage message, object existingReceiver)
        {
            return existingReceiver == null && message.ProcessingRequirements == MessageProcessingRequirements.RequiresExisting;
        }

        private static bool ExpectedNoneButFoundOne(IncomingMessage message, object existingReceiver)
        {
            return existingReceiver != null && message.ProcessingRequirements == MessageProcessingRequirements.RequiresNew;
        }

        private static IMessagingAggregateRoot CreateNewAggregateInstance(Type recieverType, Guid id)
        {
            return (IMessagingAggregateRoot)Activator.CreateInstance(recieverType, new object[] {id});
        }


    }
}
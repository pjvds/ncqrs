using System;
using Ncqrs.Domain;

namespace Ncqrs.Messaging
{
   public class MessageService : IMessageService
   {
      private readonly IReceiverResolutionStrategy _receiverResolutionStrategy;

      public MessageService(IReceiverResolutionStrategy receiverResolutionStrategy)
      {
         _receiverResolutionStrategy = receiverResolutionStrategy;
      }

      public void Process(IMessage message)
      {         
         using (var work = NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork())
         {
            var targetAggregateRoot = GetReceiver(work, message);
            targetAggregateRoot.ProcessMessage(message);            
            work.Accept();
         }
      }

      private IMessagingAggregateRoot GetReceiver(IUnitOfWorkContext work, IMessage message)
      {
         var receiverInfo = _receiverResolutionStrategy.Resolve(message.ReceiverId);
         var existingReceiver = (IMessagingAggregateRoot)work.GetById(receiverInfo.Type, receiverInfo.Id);
         CheckProcessingRequirements(message, existingReceiver);

         return existingReceiver ?? CreateNewAggregateInstance(receiverInfo.Type);
      }

      private static void CheckProcessingRequirements(IMessage message, object existingReceiver)
      {
         if (ExpectedNoneButFoundOne(message, existingReceiver) ||
             ExpectedOneButFoundNone(message, existingReceiver))
         {
            throw new MessageProcessingRequirementsViolationException(message);
         }
      }

      private static bool ExpectedOneButFoundNone(IMessage message, object existingReceiver)
      {
         return existingReceiver == null && message.ProcessingRequirements == MessageProcessingRequirements.RequiresExisting;
      }

      private static bool ExpectedNoneButFoundOne(IMessage message, object existingReceiver)
      {
         return existingReceiver != null && message.ProcessingRequirements == MessageProcessingRequirements.RequiresNew;
      }

      private static IMessagingAggregateRoot CreateNewAggregateInstance(Type recieverType)
      {
         return (IMessagingAggregateRoot)Activator.CreateInstance(recieverType);
      }

          
   }
}
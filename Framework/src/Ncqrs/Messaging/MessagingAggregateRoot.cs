using System;
using Ncqrs.Domain;

namespace Ncqrs.Messaging
{
   public class MessagingAggregateRoot : AggregateRootMappedByConvention, IMessagingAggregateRoot
   {
      private readonly MessagingContext _messagingContext = new MessagingContext();

      public MessagingContext MessagingContext
      {
         get { return _messagingContext; }
      }

      void IMessagingAggregateRoot.ProcessMessage(IMessage message)
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
            //ApplyEvent(new MessageReceivedEvent(DateTime.Now, message));
         }
         finally
         {
            MessagingContext.OnEndMessageProcessing();
         }
      }

      protected ISetMessageRequirements Reply<T>(T message)
         where T : IMessage
      {
         message.SenderId = LocalResolutionStrategy.MakeId(GetType(), Id);
         message.MessageId = NcqrsEnvironment.Get<IUniqueIdentifierGenerator>().GenerateNewId();
         message.ReceiverId = MessagingContext.MessageBeingProcessed.SenderId;
         message.RelatedMessageId = MessagingContext.MessageBeingProcessed.MessageId;
         return new FluentMessageSender(message, x => ApplyEvent(new MessageSentEvent(DateTime.Now, x)));
      }

      protected ISetMessageDestination Send<T>(T message)
         where T : IMessage
      {
         message.SenderId = LocalResolutionStrategy.MakeId(GetType(), Id);
         message.MessageId = NcqrsEnvironment.Get<IUniqueIdentifierGenerator>().GenerateNewId();
         return new FluentMessageSender(message, x => ApplyEvent(new MessageSentEvent(DateTime.Now, x)));
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
         void Process(object aggregateRoot, object message);
      }

      private static IMessageProcessor CreateProcessor(IMessage message)
      {
         Type processorType = typeof(MessageProcessor<>).MakeGenericType(message.GetType());
         return (IMessageProcessor)Activator.CreateInstance(processorType);
      }  

      private class MessageProcessor<T> : IMessageProcessor
         where T : IMessage
      {
         public void Process(object aggregateRoot, object message)
         {
            var handler = aggregateRoot as IMessageHandler<T>;
            if (handler == null)
            {
               throw new MessageMappingException(message.GetType(), aggregateRoot.GetType());
            }
            handler.Handle((T)message);
         }
      }

      private class FluentMessageSender : ISetMessageDestination, ISetMessageRequirements
      {
         private readonly IMessage _message;
         private readonly Action<IMessage> _sendAction;

         public FluentMessageSender(IMessage message, Action<IMessage> sendAction)
         {
            _message = message;
            _sendAction = sendAction;
         }

         public ISetMessageRequirements To<T>(Guid destinationId)
         {
            _message.ReceiverId = LocalResolutionStrategy.MakeId(typeof(T), destinationId);
            return this;
         }

         public void Requiring(MessageProcessingRequirements requirements)
         {
            _message.ProcessingRequirements = requirements;
            _sendAction(_message);
         }
      }
   }

   public interface ISetMessageDestination
   {
      ISetMessageRequirements To<T>(Guid destinationId);
   }

   public interface ISetMessageRequirements
   {
      void Requiring(MessageProcessingRequirements requirements);
   }
}
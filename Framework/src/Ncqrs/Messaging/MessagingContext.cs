using System;
using System.Collections.Generic;
using System.Linq;

namespace Ncqrs.Messaging
{
   public class MessagingContext
   {
      private readonly List<MessageReceivedEvent> _receivedMessages = new List<MessageReceivedEvent>();
      private readonly List<MessageSentEvent> _sentMessages = new List<MessageSentEvent>();
      private IMessage _messageBeingProcessed;

      public IMessage MessageBeingProcessed
      {
         get { return _messageBeingProcessed; }
      }

      public void OnReceived(MessageReceivedEvent @event)
      {
         _receivedMessages.Add(@event);
      }

      public void OnBeginMessageProcessing(IMessage messageBeingProcessed)
      {
         _messageBeingProcessed = messageBeingProcessed;
      }

      public void OnEndMessageProcessing()
      {
         _messageBeingProcessed = null;
      }

      public void OnSent(MessageSentEvent @event)
      {
         _sentMessages.Add(@event);
      }

      public bool WasAlreadyProcessed(IMessage messageToBeProcessed)
      {
         return _receivedMessages.Any(x => x.Message.MessageId == messageToBeProcessed.MessageId);
      }

      public IMessage GetRelatedMessage(IMessage messageBeingProcessed)
      {
         if (messageBeingProcessed.RelatedMessageId.HasValue)
         {
            var relatedEvent = _sentMessages.FirstOrDefault(x => x.Message.MessageId == messageBeingProcessed.RelatedMessageId.Value);
            return relatedEvent != null
                      ? relatedEvent.Message
                      : null;
         }
         return null;
      }

      //public IEnumerable<IMessage> GetMessagesReceivedFrom(Guid originId)
      //{         
      //   return _receivedMessages.Select(x => x.Message).Where(x => x.SenderId == originId);
      //}

      //public IEnumerable<IMessage> GetMessagesSentTo(Guid destinationId)
      //{
      //   return _sentMessages.Select(x => x.Message).Where(x => x.ReceiverId == destinationId);
      //}
   }
}
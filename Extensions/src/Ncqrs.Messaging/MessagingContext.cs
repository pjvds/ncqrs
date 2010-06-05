using System;
using System.Collections.Generic;
using System.Linq;

namespace Ncqrs.Messaging
{
   public class MessagingContext
   {
      private readonly List<MessageReceivedEvent> _receivedMessages = new List<MessageReceivedEvent>();
      private readonly List<MessageSentEvent> _sentMessages = new List<MessageSentEvent>();
      private IncomingMessage _messageBeingProcessed;

      public IncomingMessage MessageBeingProcessed
      {
         get { return _messageBeingProcessed; }
      }

      public void OnReceived(MessageReceivedEvent @event)
      {
         _receivedMessages.Add(@event);
      }

      public void OnBeginMessageProcessing(IncomingMessage messageBeingProcessed)
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

      public bool WasAlreadyProcessed(IncomingMessage messageToBeProcessed)
      {
         return _receivedMessages.Any(x => x.Message.MessageId == messageToBeProcessed.MessageId);
      }

      public object GetRelatedMessage(IncomingMessage messageBeingProcessed)
      {
         if (messageBeingProcessed.RelatedMessageId.HasValue)
         {
            var relatedEvent = _sentMessages.FirstOrDefault(x => x.Message.MessageId == messageBeingProcessed.RelatedMessageId.Value);
            return relatedEvent != null
                      ? relatedEvent.Message.Payload
                      : null;
         }
         return null;
      }

      //public IEnumerable<IIncomingMessage> GetMessagesReceivedFrom(Guid originId)
      //{         
      //   return _receivedMessages.Select(x => x.Message).Where(x => x.SenderId == originId);
      //}

      //public IEnumerable<IIncomingMessage> GetMessagesSentTo(Guid destinationId)
      //{
      //   return _sentMessages.Select(x => x.Message).Where(x => x.ReceiverId == destinationId);
      //}
   }
}
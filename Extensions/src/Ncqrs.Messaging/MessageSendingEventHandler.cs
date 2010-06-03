using System;
using System.Collections.Generic;
using System.Linq;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Messaging
{
   public class MessageSendingEventHandler : IEventHandler<MessageSentEvent>
   {
      private readonly List<IMessageSender> _sendersInOrder = new List<IMessageSender>();

      public void AddSender(IMessageSender sender)
      {
         _sendersInOrder.Add(sender);
      }

      public void Handle(MessageSentEvent @event)
      {
         if (_sendersInOrder.Any(messageSender => messageSender.TrySend(@event.Message)))
         {
            return;
         }
         throw new InvalidOperationException();
      }
   }
}
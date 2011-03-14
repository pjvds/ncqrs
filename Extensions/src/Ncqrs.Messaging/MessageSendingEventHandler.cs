using System;
using System.Collections.Generic;
using System.Linq;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Messaging
{
   public class MessageSendingEventHandler : IEventHandler<MessageSentEvent>
   {
      private readonly List<ConditionalSendingStrategy> _strategies = new List<ConditionalSendingStrategy>();

      public void UseStrategy(ConditionalSendingStrategy conditionalSendingStrategy)
      {
         _strategies.Add(conditionalSendingStrategy);
      }

      public void Handle(IPublishedEvent<MessageSentEvent> @event)
      {
         if (_strategies.Any(messageSender => messageSender.Send(@event.Payload.Message)))
         {
            return;
         }
         throw new InvalidOperationException();
      }
   }
}
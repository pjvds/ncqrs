using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain;
using Sample.Events;
using Ncqrs.Eventing.Mapping;

namespace Sample.Domain
{
    public class Message : AggregateRoot
    {
        private String _messageText;

        public Message(Guid messageId, String text)
        {
            var e = new NewMessageAdded
            {
                MessageId = messageId,
                Text = text
            };

            ApplyEvent(e);
        }

        [EventHandler]
        private void NewMessageCreatedEventHandler(NewMessageAdded e)
        {
            OverrideId(e.MessageId);
            _messageText = e.Text;
        }
    }
}
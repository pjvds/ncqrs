using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain;
using Sample.Events;
using Ncqrs.Eventing.Mapping;
using System.Diagnostics.Contracts;

namespace Sample.Domain
{
    public class Message : AggregateRoot
    {
        private String _messageText;
        private DateTime _creationDate;

        public Message(Guid messageId, String text)
        {
            var e = new NewMessageAdded
            {
                MessageId = messageId,
                Text = text,
                CreationDate = DateTime.UtcNow
            };

            ApplyEvent(e);
        }

        public void UpdateMessageText(String text)
        {
            var e = new MessageTextUpdated
            {
                MessageId = Id,
                UpdatedMessageText = text
            };

            ApplyEvent(e);
        }

        [EventHandler]
        private void NewMessageCreatedEventHandler(NewMessageAdded e)
        {
            OverrideId(e.MessageId);
            _messageText = e.Text;
            _creationDate = e.CreationDate;
        }

        [EventHandler]
        private void MessageTextUpdatedEventHandler(MessageTextUpdated e)
        {
            Contract.Assert(e.MessageId == Id, "The MessageId should be the same as "+
                                               "the Id of this instance. Since otherwise, "+
                                               "the event is not owned by this instance and "+
                                               "should never reach this point.");

            _messageText = e.UpdatedMessageText;
        }
    }
}
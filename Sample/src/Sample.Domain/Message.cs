using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain;
using Sample.Events;
using Ncqrs.Domain.Mapping;
using System.Diagnostics.Contracts;
using Ncqrs.Domain;

namespace Sample.Domain
{
    public class Message : AggregateRootMappedByConvention
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

        protected Message(IEnumerable<HistoricalEvent> history)
            : base(history)
        {
        }

        public void UpdateMessageText(String newMessageText)
        {
            var e = new MessageTextUpdated
            {
                MessageId = Id,
                UpdatedMessageText = newMessageText,
                ChangeDate = DateTime.UtcNow
            };

            ApplyEvent(e);
        }

        private void OnNewMessageAdded(NewMessageAdded e)
        {
             OverrideId(e.MessageId);
            _messageText = e.Text;
            _creationDate = e.CreationDate;
        }

        private void OnMessageTextUpdated(MessageTextUpdated e)
        {
            Contract.Assert(e.MessageId == Id, "The MessageId should be the same as "+
                                               "the Id of this instance. Since otherwise, "+
                                               "the event is not owned by this instance and "+
                                               "should never reach this point.");

            _messageText = e.UpdatedMessageText;
        }
    }
}
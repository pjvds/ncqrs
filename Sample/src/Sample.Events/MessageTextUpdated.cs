using System;
using Ncqrs.Domain;

namespace Sample.Events
{
    public class MessageTextUpdated : DomainEvent
    {
        public Guid MessageId
        {
            get;
            set;
        }

        public String UpdatedMessageText
        {
            get;
            set;
        }

        public DateTime ChangeDate
        {
            get;
            set;
        }
    }
}

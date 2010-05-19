using System;
using Ncqrs.Domain;

namespace Sample.Events
{
    [Serializable]
    public class NewMessageAdded : DomainEvent
    {
        public Guid MessageId
        {
            get;
            set;
        }

        public String Text
        {
            get;
            set;
        }

        public DateTime CreationDate
        {
            get;
            set;
        }
    }
}

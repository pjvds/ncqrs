using System;
using Ncqrs.Eventing;

namespace Sample.Events
{
    public class NewMessageAdded : IEvent
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

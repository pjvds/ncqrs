using System;
using Ncqrs.Eventing;

namespace Sample.Events
{
    public class MessageTextUpdated : IEvent
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

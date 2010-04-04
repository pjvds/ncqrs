using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

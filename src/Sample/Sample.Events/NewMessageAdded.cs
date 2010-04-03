using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

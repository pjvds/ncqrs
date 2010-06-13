using System;
using System.Runtime.Serialization;
using Ncqrs.Eventing;

namespace Ncqrs.Domain
{
    [Serializable]
    public class EventDataNotHandledException : Exception
    {
        public IEvent Event
        {
            get;
            private set;
        }

        public EventDataNotHandledException(IEvent evnt)
            : this(evnt, evnt != null ? String.Format("No handler handled the {0} event data.", evnt.GetType().FullName) : null)
        {

        }

        public EventDataNotHandledException(IEvent evnt, String message)
            : this(evnt, message, null)
        {
        }

        public EventDataNotHandledException(IEvent evnt, String message, Exception inner)
            : base(message, inner)
        {
            Event = evnt;
        }

        protected EventDataNotHandledException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}

using System;
using System.Runtime.Serialization;

namespace Ncqrs.Eventing.ServiceModel.Bus
{
    [Serializable]
    public class EventNotHandledException : Exception
    {
        public IEvent Event
        {
            get;
            private set;
        }

        public EventNotHandledException(IEvent evnt)
            : this(evnt, evnt != null ? String.Format("No handler handled the {0} event.", evnt.GetType().FullName) : null)
        {

        }

        public EventNotHandledException(IEvent evnt, String message)
            : this(evnt, message, null)
        {
        }

        public EventNotHandledException(IEvent evnt, String message, Exception inner)
            : base(message, inner)
        {
            Event = evnt;
        }

        protected EventNotHandledException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}

using System;
using System.Runtime.Serialization;
using Ncqrs.Eventing;

namespace Ncqrs.Domain
{
    [Serializable]
    public class EventDataNotHandledException : Exception
    {
        public IEventData EventData
        {
            get;
            private set;
        }

        public EventDataNotHandledException(IEventData eventData)
            : this(eventData, eventData != null ? String.Format("No handler handled the {0} event data.", eventData.GetType().FullName) : null)
        {

        }

        public EventDataNotHandledException(IEventData eventData, String message)
            : this(eventData, message, null)
        {
        }

        public EventDataNotHandledException(IEventData eventData, String message, Exception inner)
            : base(message, inner)
        {
            EventData = eventData;
        }

        protected EventDataNotHandledException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}

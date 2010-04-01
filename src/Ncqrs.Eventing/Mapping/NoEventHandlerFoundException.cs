using System;

namespace Ncqrs.Eventing.Mapping
{
    public class NoEventHandlerFoundException : Exception
    {
        public IEvent Event
        {
            get;
            private set;
        }

        public NoEventHandlerFoundException(IEvent evnt)
            : this(String.Format("No handler found for event {0}.", evnt), evnt)
        {

        }

        public NoEventHandlerFoundException(string message, IEvent evnt)
            : this(message, evnt, null)
        {
        }

        public NoEventHandlerFoundException(string message, IEvent evnt, Exception innerException)
            : base(message, innerException)
        {
            Event = evnt;
        }
    }
}

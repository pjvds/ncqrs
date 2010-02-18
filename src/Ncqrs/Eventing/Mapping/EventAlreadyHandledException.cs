using System;
using System.Runtime.Serialization;

namespace Ncqrs.Eventing.Mapping
{
    public class EventAlreadyHandledException : Exception
    {
        public EventAlreadyHandledException(string message)
            : base(message)
        {
        }

        public EventAlreadyHandledException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected EventAlreadyHandledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

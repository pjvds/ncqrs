using System;

namespace Ncqrs.Eventing.Storage
{
    public abstract class StoredEvent
    {
        public StoredEvent(Guid eventIdentifier, DateTime eventTimeStamp, string eventName, Version eventVersion, Guid eventSourceId, long eventSequence)
        {
            EventIdentifier = eventIdentifier;
            EventTimeStamp = eventTimeStamp;
            EventName = eventName;
            EventVersion = eventVersion;
            EventSourceId = eventSourceId;
            EventSequence = eventSequence;
        }

        public Guid EventIdentifier { get; private set; }
        public DateTime EventTimeStamp { get; private set; }

        /// <summary>
        ///   Gets or sets the event name of the original object from which this document was constructed.
        /// </summary>
        public string EventName { get; private set; }
        public Version EventVersion { get; set; }

        public Guid EventSourceId { get; private set;}
        public long EventSequence { get; private set; }
    }

    public class StoredEvent<T> : StoredEvent
    {
        public StoredEvent(Guid eventIdentifier, DateTime eventTimeStamp, string eventName, Version eventVersion, Guid eventSourceId, long eventSequence, T data)
            : base(eventIdentifier,eventTimeStamp,eventName,eventVersion,eventSourceId,eventSequence)
        {
            Data = data;
        }

        public T Data { get; private set; }

        public StoredEvent<TOther> Clone<TOther>(TOther data)
        {
            return new StoredEvent<TOther>(EventIdentifier, EventTimeStamp, EventName, EventVersion, EventSourceId, EventSequence, data);
        }
    }
}

using System;
using System.Globalization;
using System.Reflection;

namespace Ncqrs.Eventing.Sourcing
{
    [Serializable]
    public class SourcedEvent<TEventData> : EventBase<TEventData>, ISourcedEvent<TEventData> where TEventData : IEventData
    {
        /// <summary>
        /// Gets the id of the event source that caused the event.
        /// </summary>
        /// <value>The id of the event source that caused the event.</value>
        public Guid EventSourceId
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the event sequence number.
        /// </summary>
        /// <remarks>
        /// An sequence of events always starts with <c>0</c>. So the first event in a sequence has the <see cref="EventSequence"/> value of <c>0</c>.
        /// </remarks>
        /// <value>A number that represents the order of where this events occurred in the sequence.</value>
        public long EventSequence
        {
            get;
            internal set;
        }

        private SourcedEvent(Guid eventSourceId, long eventSequence, TEventData eventData) : base(eventData)
        {
            EventSourceId = eventSourceId;
            EventSequence = eventSequence;
        }

        internal static ISourcedEvent<IEventData> Create(Guid eventSourceId, long eventSequence, IEventData eventData)
        {
            var eventDataType = eventData.GetType();
            var sourcedEventType = typeof (SourcedEvent<>).MakeGenericType(eventDataType);

            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            return (ISourcedEvent<IEventData>)Activator.CreateInstance(sourcedEventType, flags, null, new object[] { eventSourceId, eventSequence, eventData }, CultureInfo.InvariantCulture);
        }
    }
}
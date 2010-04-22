using System;

namespace Ncqrs.Domain
{
    /// <summary>
    /// Represents an event that has already happend.
    /// </summary>
    public class HistoricalEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HistoricalEvent"/> class.
        /// </summary>
        /// <param name="timeStamp">The moment in time that the event happened.</param>
        /// <param name="evnt">The event that happened.</param>
        /// <exception cref="ArgumentNullException">Thrown when <i>evnt</i> is null.</exception>
        public HistoricalEvent(DateTime timeStamp, IEvent evnt)
        {
            if (evnt == null) throw new ArgumentNullException("evnt");
            if (timeStamp == DateTime.MinValue || timeStamp == DateTime.MaxValue) throw new ArgumentOutOfRangeException("timeStamp", "An event should accored on a point in time, otherwise then DateTime.MinValue or DateTime.MaxValue.");

            TimeStamp = timeStamp;
            Event = evnt;
        }

        /// <summary>
        /// Gets the moment in time that the event happened.
        /// </summary>
        public DateTime TimeStamp { get; private set; }

        /// <summary>
        /// Gets the event.
        /// </summary>
        public IEvent Event { get; private set; }
    }
}

using System;

namespace Ncqrs.Eventing.Sourcing.Snapshotting
{
    /// <summary>
    /// Holds the full state of an <see cref="IEventSource"/> at a certain version.
    /// </summary>
    [Serializable]
    public abstract class Snapshot : ISnapshot
    {
        /// <summary>
        /// Gets the id of the event source from which this snapshot was created.
        /// </summary>
        /// <remarks>
        /// The id of the event source from which this snapshot was created.
        /// </remarks>
        public Guid EventSourceId
        {
            get; set;
        }


        /// <summary>
        /// Gets the version of the event source when this snapshot was created.
        /// </summary>
        /// <value>The version of the event source when this snapshot was created.</value>
        public long EventSourceVersion
        {
            get; set;
        }
    }
}

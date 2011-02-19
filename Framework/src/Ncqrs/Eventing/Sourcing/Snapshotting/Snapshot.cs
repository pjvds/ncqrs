using System;

namespace Ncqrs.Eventing.Sourcing.Snapshotting
{
    /// <summary>
    /// Holds the full state of an aggregate root at a certain version.
    /// </summary>
    [Serializable]
    public class Snapshot
    {
        /// <summary>
        /// Initializes a new instance of the Snapshot class.
        /// </summary>
        /// <param name="eventSourceId">The value which uniquely identifies the stream to which the snapshot applies.</param>
        /// <param name="version">The position at which the snapshot applies.</param>
        /// <param name="payload">The snapshot or materialized view of the stream at the revision indicated.</param>
        public Snapshot(Guid eventSourceId, long version, object payload)
            : this()
        {
            EventSourceId = eventSourceId;
            Version = version;
            Payload = payload;
        }

        /// <summary>
        /// Initializes a new instance of the Snapshot class.
        /// </summary>
        protected Snapshot()
        {
        }

        /// <summary>
        /// Gets the value which uniquely identifies the aggregate root to which the snapshot applies.
        /// </summary>
        public Guid EventSourceId { get; private set; }

        /// <summary>
        /// Gets the position at which the snapshot applies.
        /// </summary>
        public long Version { get; private set; }

        /// <summary>
        /// Gets the snapshot or materialized view of the stream at the revision indicated.
        /// </summary>
        public object Payload { get; private set; }
    }
}

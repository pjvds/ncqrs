using System;

namespace Ncqrs.Eventing.Sourcing.Snapshotting
{
    /// <summary>
    /// Holds the full state of an <see cref="IEventSource"/> at a certain version.
    /// </summary>
    public interface ISnapshot
    {
        /// <summary>
        /// Gets the event source id.
        /// </summary>
        /// <value>The event source id.</value>
        Guid EventSourceId
        {
            get;
        }

        /// <summary>
        /// Gets the version of the event source from when this snapshot was created.
        /// </summary>
        /// <value>The event source version.</value>
        long EventSourceVersion
        {
            get;
        }
    }
}
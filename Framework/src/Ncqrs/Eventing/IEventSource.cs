using System;
using System.Collections.Generic;

namespace Ncqrs.Eventing
{
    /// <summary>
    /// An object that represents all his state changes via a sequence of events.
    /// </summary>
    public interface IEventSource
    {
        /// <summary>
        /// Gets the globally unique identifier.
        /// </summary>
        Guid Id
        {
            get;
        }

        /// <summary>
        /// Gets the version of this source.
        /// </summary>
        long Version
        { 
            get;
        }

        /// <summary>
        /// Gets the uncommitted events.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISourcedEvent> GetUncommittedEvents();

        /// <summary>
        /// Commits the events.
        /// </summary>
        void AcceptChanges();
    }
}
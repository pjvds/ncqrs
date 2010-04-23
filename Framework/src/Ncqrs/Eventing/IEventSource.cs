using System;
using System.Collections.Generic;

namespace Ncqrs.Eventing
{
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
        IEnumerable<IEventSourcedEvent> GetUncommittedEvents();

        /// <summary>
        /// Commits the events.
        /// </summary>
        void CommitEvents();
    }
}
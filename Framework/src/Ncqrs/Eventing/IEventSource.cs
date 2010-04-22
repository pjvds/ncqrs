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

        IEnumerable<IEvent> GetUncommitedEvents();
    }
}
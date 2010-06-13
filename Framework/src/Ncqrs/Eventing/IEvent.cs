using System;
using System.Diagnostics.Contracts;

namespace Ncqrs.Eventing
{
    /// <summary>
    /// Represents an event.
    /// </summary>
    [ContractClass(typeof(IEventContracts))]
    public interface IEvent
    {
        /// <summary>
        /// Gets the unique identifier for this event.
        /// </summary>
        /// <value>A <see cref="Guid"/> that represents the unique identifier of this event.</value>
        Guid EventIdentifier
        {
            get;
        }

        /// <summary>
        /// Gets the time stamp for this event.
        /// </summary>
        /// <value>a <see cref="DateTime"/> UTC value that represents the point
        /// in time where this event occurred.</value>
        DateTime EventTimeStamp
        {
            get;
        }
    }

    [ContractClassFor(typeof(IEvent))]
    internal sealed class IEventContracts : IEvent
    {
        public Guid EventIdentifier
        {
            get
            {
                return default(Guid);
            }
        }

        public DateTime EventTimeStamp
        {
            get
            {
                Contract.Ensures(Contract.Result<DateTime>().Kind == DateTimeKind.Utc, "The event time stamp should always be in a UTC kind.");

                return default(DateTime);
            }
        }
    }
}
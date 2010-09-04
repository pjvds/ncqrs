using System;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Sourcing.Mapping;

namespace Ncqrs.Domain
{
    /// <summary>
    /// The abstract concept of an aggregate root.
    /// </summary>
    public abstract class AggregateRoot : EventSource
    {
        /// <summary>
        /// Occurs when an event was applied to an <see cref="AggregateRoot"/>.
        /// </summary>
        internal static event EventHandler<EventAppliedArgs> EventApplied;

        protected AggregateRoot()
        {}

        protected AggregateRoot(Guid id) : base(id)
        {}

        [NoEventHandler]
        protected override void OnEventApplied(SourcedEvent appliedEvent)
        {
            if(EventApplied != null)
            {
                EventApplied(this, new EventAppliedArgs(appliedEvent));
            }
        }
    }
}
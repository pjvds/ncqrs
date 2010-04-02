using System;
using Ncqrs.Eventing.Mapping;
using System.Collections.Generic;
using Ncqrs.Eventing;
using Ncqrs.Domain.Storage;

namespace Ncqrs.Domain
{
    /// <summary>
    /// The abstract concept of an aggregate root.
    /// </summary>
    public abstract class AggregateRoot : MappedEventSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
        /// </summary>
        /// <remarks>
        /// This instance will be initialized with the <see cref="BasicGuidGenerator"/>.
        /// </remarks>
        protected AggregateRoot() : this(new BasicGuidGenerator())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
        /// </summary>
        /// <param name="idGenerator">The id generator that will be used to generate a new id for this instance.</param>
        protected AggregateRoot(IUniqueIdentifierGenerator idGenerator) : base(idGenerator)
        {
        }

        protected AggregateRoot(IEnumerable<HistoricalEvent> history) : base(history)
        {
        }

        protected override void OnEventApplied(IEvent evnt)
        {
            // Register this instance as a dirty one.
            UnitOfWork.Current.RegisterDirtyInstance(this);

            // Call base.
            base.OnEventApplied(evnt);
        }
    }
}
using System;
using Ncqrs.Eventing.Mapping;
using System.Collections.Generic;
using Ncqrs.Eventing;
using Ncqrs.Domain.Storage;

namespace Ncqrs.Domain
{
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

        protected AggregateRoot(IEnumerable<HistoricalEvent> history) : this(new BasicGuidGenerator()) // TODO: Is id generator needed?
        {
            InitializeFromHistory(history);
        }
    }
}
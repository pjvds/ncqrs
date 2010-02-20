using System;
using Ncqrs.Eventing.Mapping;
using System.Collections.Generic;
using Ncqrs.Eventing;
using Ncqrs.Domain.Storage;

namespace Ncqrs.Domain
{
    public abstract class AggregateRoot : MappedEventSource
    {
        protected IDomainRepository DomainRepository // TODO: Are we sure we want to expose the repository this way?
        {
            get
            {
                return UnitOfWork.Current.DomainRepository;
            }
        }

        protected AggregateRoot() : this(new BasicGuidGenerator())
        {
        }

        protected AggregateRoot(IUniqueIdentifierGenerator idGenerator) : base(idGenerator)
        {
        }

        protected AggregateRoot(IEnumerable<HistoricalEvent> history) : this(new BasicGuidGenerator()) // TODO: Is id generator needed?
        {
            InitializeFromHistory(history);
        }
    }
}
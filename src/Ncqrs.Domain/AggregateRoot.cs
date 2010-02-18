using System;
using Ncqrs.Eventing.Mapping;
using System.Collections.Generic;
using Ncqrs.Eventing;

namespace Ncqrs.Domain
{
    public abstract class AggregateRoot : MappedEventSource
    {
        protected AggregateRoot() : base()
        {
        }

        protected AggregateRoot(IEnumerable<HistoricalEvent> history) : this()
        {
            InitializeFromHistory(history);
        }
    }
}
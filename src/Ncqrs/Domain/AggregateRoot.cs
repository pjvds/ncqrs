using System;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Mapping;
using System.Collections.Generic;

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
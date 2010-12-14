using System;
using System.Collections.Generic;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage.SQL;

namespace Ncqrs.EventBus
{
    public class MsSqlServerBrowsableEventStore : IBrowsableEventStore
    {
        private readonly MsSqlServerEventStore _wrappedStore;
        private Guid? _lastEventId;

        public MsSqlServerBrowsableEventStore(MsSqlServerEventStore wrappedStore)
        {
            _wrappedStore = wrappedStore;
        }

        public void SetCursorPositionAfter(Guid? lastEventId)
        {
            _lastEventId = lastEventId;
        }

        public IEnumerable<SourcedEvent> FetchEvents(int maxCount)
        {
            var result = _wrappedStore.GetEventsAfter(_lastEventId, maxCount);

            foreach (var evnt in result)
            {
                _lastEventId = evnt.EventIdentifier;
                yield return evnt;
            }
        }
    }
}
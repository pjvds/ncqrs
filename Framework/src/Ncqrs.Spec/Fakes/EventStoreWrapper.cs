using System;
using System.Collections.Generic;
using System.Linq;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Storage;

namespace Ncqrs.Spec.Fakes
{

    public class EventStoreWrapper : IEventStore
    {

        private readonly IEventStore _realStore;
        private IEventStore _testStore;
        private readonly HashSet<Guid> _underTest;

        public EventStoreWrapper()
            : this(NcqrsEnvironment.Get<IEventStore>())
        {
        }
        
        public EventStoreWrapper(IEventStore realStore)
        {
            _realStore = realStore;
            _underTest = new HashSet<Guid>();
            _testStore = new InMemoryEventStore();
        }

        public CommittedEventStream ReadFrom(Guid id, long minVersion, long maxVersion)
        {
            return _underTest.Contains(id)
                       ? _testStore.ReadFrom(id, minVersion, maxVersion)
                       : _realStore.ReadFrom(id, minVersion, maxVersion);
        }

        public void Store(UncommittedEventStream eventStream)
        {
            Store(eventStream.CommitId, eventStream);
        }

        private void Store(Guid commitId, IEnumerable<UncommittedEvent> events)
        {
            var eventsForTestStore = events.Where(e => _underTest.Contains(e.EventSourceId));
            var eventsForRealStore = events.Except(eventsForTestStore);

            if (eventsForTestStore.Any())
                _testStore.Store(BuildStream(commitId, eventsForTestStore));

            if (eventsForRealStore.Any())
                _realStore.Store(BuildStream(commitId, eventsForRealStore));
        }

        private UncommittedEventStream BuildStream(Guid commitId, IEnumerable<UncommittedEvent> events)
        {
            var stream = new UncommittedEventStream(commitId);
            foreach (var evnt in events)
                stream.Append(evnt);
            return stream;
        }

        public void Given(UncommittedEventStream history)
        {
            var historyBySources = history.GroupBy(e => e.EventSourceId);
            foreach (var sourceHistory in historyBySources)
            {
                _underTest.Add(sourceHistory.Key);
                Store(history.CommitId, sourceHistory);
            }
        }

        public void Clear()
        {
            _testStore = new InMemoryEventStore();
            _underTest.Clear();
        }

    }
}


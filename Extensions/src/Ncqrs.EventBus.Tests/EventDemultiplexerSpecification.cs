using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.EventBus.Tests
{
    [TestFixture]
    public class EventDemultiplexerSpecification
    {
        [Test]
        public void When_all_queues_are_empty_event_is_fetched_from_store()
        {
            Guid eventSourceId = Guid.NewGuid();
            var eventStore = new FakeEventStore(
                CreateEvent(eventSourceId));
            var sut = new EventDemultiplexer(eventStore);

            sut.GetNext();

            eventStore.AssertWasFetched(1);
        }

        [Test]
        public void When_one_queue_is_blocked_event_is_fetched_to_another_queue()
        {
            Guid firstEventSourceId = Guid.NewGuid();
            Guid secondEventSourceId = Guid.NewGuid();
            var eventStore = new FakeEventStore(
                CreateEvent(firstEventSourceId),
                CreateEvent(secondEventSourceId));

            var sut = new EventDemultiplexer(eventStore);
            sut.GetNext();

            sut.GetNext();

            eventStore.AssertWasFetched(2);
        }

        [Test]
        public void When_one_queue_is_blocked_and_next_event_belongs_to_same_aggregate_yet_another_event_is_fetched_to_another_queue()
        {
            Guid firstEventSourceId = Guid.NewGuid();
            Guid secondEventSourceId = Guid.NewGuid();

            var eventStore = new FakeEventStore(
                CreateEvent(firstEventSourceId),
                CreateEvent(firstEventSourceId),
                CreateEvent(secondEventSourceId));

            var sut = new EventDemultiplexer(eventStore);
            sut.GetNext();

            sut.GetNext();

            eventStore.AssertWasFetched(3);
        }

        private static SequencedEvent CreateEvent(Guid sourceId)
        {
            return new SequencedEvent(0, new TestEvent(Guid.NewGuid(), sourceId, 0, DateTime.Now));
        }

        private class FakeEventStore : IEventStore
        {
            private readonly SequencedEvent[] _events;
            private int _index = -1;

            public FakeEventStore(params SequencedEvent[] events)
            {
                _events = events;
            }

            public void AssertWasFetched(int count)
            {
                Assert.IsTrue(_index+1 == count);
            }

            public void SetCursorPositionAfter(Guid lastEventId)
            {
                throw new NotImplementedException();
            }

            public SequencedEvent GetNext()
            {
                _index++;
                return _events[_index];
            }

            public void UnblockSource(Guid eventSourceId)
            {
                throw new NotImplementedException();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Sourcing;
using NUnit.Framework;
using Ncqrs.Eventing.Storage;

namespace Ncqrs.Tests.Eventing.Storage
{
    [TestFixture]
    public class InMemoryEventStoreSpecs
    {        
        public class SomethingDoneEvent
        {
        }

        [Test]
        public void When_getting_all_event_from_a_non_existing_event_source_the_result_should_be_empty()
        {
            var eventSourceId = Guid.NewGuid();
            var store = new InMemoryEventStore();

            var events = store.ReadFrom(eventSourceId, long.MinValue, long.MaxValue);

            events.Should().NotBeNull();
            events.Should().BeEmpty();
        }

        [Test]
        public void When_getting_all_event_from_an_existing_event_source_the_result_should_be_all_events_stored_for_that_event_source()
        {
            var eventSourceId = Guid.NewGuid();

            var stream1 = new UncommittedEventStream(Guid.NewGuid());
            stream1.Append(new UncommittedEvent(Guid.NewGuid(), eventSourceId, 1, 0, DateTime.UtcNow, new object(), new Version(1, 0)));
            stream1.Append(new UncommittedEvent(Guid.NewGuid(), eventSourceId, 2, 0, DateTime.UtcNow, new object(), new Version(1, 0)));

            var stream2 = new UncommittedEventStream(Guid.NewGuid());
            stream2.Append(new UncommittedEvent(Guid.NewGuid(), eventSourceId, 3, 1, DateTime.UtcNow, new object(), new Version(1, 0)));
            stream2.Append(new UncommittedEvent(Guid.NewGuid(), eventSourceId, 4, 1, DateTime.UtcNow, new object(), new Version(1, 0)));
            stream2.Append(new UncommittedEvent(Guid.NewGuid(), eventSourceId, 5, 1, DateTime.UtcNow, new object(), new Version(1, 0)));

            var store = new InMemoryEventStore();

            store.Store(stream1);
            store.Store(stream2);

            var events = store.ReadFrom(eventSourceId, long.MinValue, long.MaxValue);

            events.Count().Should().Be(5);
        }
    }
}

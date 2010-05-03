using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Ncqrs.Eventing.Storage;
using Ncqrs.Domain;
using Ncqrs.Eventing;

namespace Ncqrs.Tests.Eventing.Storage
{
    [TestFixture]
    public class InMemoryEventStoreSpecs
    {
        public class EventSourceMock : IEventSource
        {
            public Func<IEnumerable<ISourcedEvent>> GetUncommittedEventsStub;

            public Guid Id
            {
                get; set;
            }

            public long Version
            {
                get; set;
            }

            public IEnumerable<ISourcedEvent> GetUncommittedEvents()
            {
                return GetUncommittedEventsStub();
            }

            /// <summary>
            /// Commits the events.
            /// </summary>
            public void AcceptChanges()
            {
            }
        }

        public class SomethingDoneEvent : DomainEvent
        {
            public SomethingDoneEvent(Guid sourceId)
            {
                GetType().GetProperty("AggregateRootId").SetValue(this, sourceId, null);
            }
        }

        [Test]
        public void When_getting_all_event_from_a_non_existing_event_source_the_result_should_be_empty()
        {
            var eventSourceId = Guid.NewGuid();
            var store = new InMemoryEventStore();

            var events = store.GetAllEventsForEventSource(eventSourceId);

            events.Should().NotBeNull();
            events.Should().BeEmpty();
        }

        [Test]
        public void When_getting_all_event_from_an_existing_event_source_the_result_should_be_all_events_stored_for_that_event_source()
        {
            var eventSourceId = Guid.NewGuid();
            var mock = new EventSourceMock();
            mock.Id = eventSourceId;

            var store = new InMemoryEventStore();

            var events1 = new[]{
                                  new SomethingDoneEvent(eventSourceId), new SomethingDoneEvent(eventSourceId),
                              };

            
            var events2 = new[]{
                                  new SomethingDoneEvent(eventSourceId), new SomethingDoneEvent(eventSourceId), new SomethingDoneEvent(eventSourceId)
                              };

            mock.GetUncommittedEventsStub = () => events1;
            store.Save(mock);

            mock.GetUncommittedEventsStub = () => events2;
            store.Save(mock);

            var events = store.GetAllEventsForEventSource(eventSourceId);
            var unionOfStoredEvents = events1.Union(events2);

            events.Count().Should().Be(unionOfStoredEvents.Count());
            events.Should().BeEquivalentTo(unionOfStoredEvents);
        }
    }
}

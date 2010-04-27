using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Rhino.Mocks;
using Ncqrs.Domain;

namespace Ncqrs.Eventing.Storage.WindowsAzure.Tests
{
    [Serializable]
    public class FooEvent : ISourcedEvent
    {
        /// <summary>
        /// Gets the unique identifier for this event.
        /// </summary>
        public Guid EventIdentifier { get; private set; }

        /// <summary>
        /// Gets the time stamp for this event.
        /// </summary>
        /// <value>a <see cref="DateTime"/> UTC value that represents the point
        /// in time where this event occurred.</value>
        public DateTime EventTimeStamp { get; private set; }
        public Guid EventSourceId { get; private set; }
        public long EventSequence { get; private set; }

        public FooEvent(Guid eventSourceId, long eventSequence)
        {
            EventIdentifier = Guid.NewGuid();
            EventTimeStamp = DateTime.UtcNow;
            EventSourceId = eventSourceId;
            EventSequence = eventSequence;
        }
    }

    [Serializable]
    public class BarEvent : ISourcedEvent
    {
        /// <summary>
        /// Gets the unique identifier for this event.
        /// </summary>
        public Guid EventIdentifier { get; private set; }

        /// <summary>
        /// Gets the time stamp for this event.
        /// </summary>
        /// <value>a <see cref="DateTime"/> UTC value that represents the point
        /// in time where this event occurred.</value>
        public DateTime EventTimeStamp { get; private set; }
        public Guid EventSourceId { get; private set; }
        public long EventSequence { get; private set; }

        public BarEvent(Guid eventSourceId, long eventSequence)
        {
            EventIdentifier = Guid.NewGuid();
            EventTimeStamp = DateTime.UtcNow;
            EventSourceId = eventSourceId;
            EventSequence = eventSequence;
        }
    }

    [TestFixture]
    public class AzureEventStoreSpecs
    {
        [Test]
        public void All_the_events_from_a_new_event_source_should_add_events_to_event_store()
        {
            try
            {
                var mock = MockRepository.GenerateMock<IEventSource>();
                mock.Stub(m => mock.Id).Return(Guid.NewGuid()).Repeat.Any();
                mock.Stub(m => mock.GetUncommittedEvents()).Return(new ISourcedEvent[] { new FooEvent(mock.Id, 1), new BarEvent(mock.Id, 2) });

                var theEventStore = new AzureEventStore();
                theEventStore.Save(mock);

                var storedEvents = theEventStore.GetAllEventsForEventSource(mock.Id);
                storedEvents.Count().Should().Be(2);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [Test]
        public void All_the_events_from_a_event_source_should_add_events_to_event_store()
        {
            try
            {
                var mock = MockRepository.GenerateMock<IEventSource>();
                mock.Stub(m => mock.Id).Return(Guid.NewGuid()).Repeat.Any();
                mock.Stub(m => mock.GetUncommittedEvents()).Return(new ISourcedEvent[] { new FooEvent(mock.Id, 1), new BarEvent(mock.Id, 2) });

                var theEventStore = new AzureEventStore();
                theEventStore.Save(mock);

                mock.Stub(m => mock.Version).Return(2);
                mock.Stub(m => mock.GetUncommittedEvents()).Return(new ISourcedEvent[] { new FooEvent(mock.Id, 3), new BarEvent(mock.Id, 4) });
                theEventStore.Save(mock);


                var storedEvents = theEventStore.GetAllEventsForEventSource(mock.Id);
                storedEvents.Count().Should().Be(4);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}

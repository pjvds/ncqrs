using System;
using NUnit.Framework;
using FluentAssertions;

namespace Ncqrs.EventBus.Tests
{
    [TestFixture]
    public class EventDemultiplexerSpecification
    {
        [Test]
        public void When_event_source_is_not_blocked_event_is_passed_through_system()
        {
            Guid eventSourceId = Guid.NewGuid();
            int enqueuedToProcessingCount = 0;

            var sut = new Demultiplexer();
            sut.EventDemultiplexed += (s, e) => { enqueuedToProcessingCount++; };
            sut.Demultiplex(CreateEvent(eventSourceId));

            enqueuedToProcessingCount.Should().Be(1);

        }

        [Test]
        public void Different_event_sources_does_not_block_each_other()
        {
            Guid firstEventSourceId = Guid.NewGuid();
            Guid secondEventSourceId = Guid.NewGuid();

            int enqueuedToProcessingCount = 0;

            var sut = new Demultiplexer();
            sut.EventDemultiplexed += (s, e) => { enqueuedToProcessingCount++; };
            sut.Demultiplex(CreateEvent(firstEventSourceId));
            sut.Demultiplex(CreateEvent(secondEventSourceId));

            enqueuedToProcessingCount.Should().Be(2);
        }

        [Test]
        public void When_event_source_is_blocked_event_is_enqueued()
        {
            Guid eventSourceId = Guid.NewGuid();

            int enqueuedToProcessingCount = 0;

            var sut = new Demultiplexer();
            sut.EventDemultiplexed += (s, e) => { enqueuedToProcessingCount++; };
            sut.Demultiplex(CreateEvent(eventSourceId));
            sut.Demultiplex(CreateEvent(eventSourceId));

            enqueuedToProcessingCount.Should().Be(1);
        }

        private static SourcedEventProcessingElement CreateEvent(Guid sourceId)
        {
            return new SourcedEventProcessingElement(0, new TestEvent(Guid.NewGuid(), sourceId, 0, DateTime.Now));
        }
    }
}
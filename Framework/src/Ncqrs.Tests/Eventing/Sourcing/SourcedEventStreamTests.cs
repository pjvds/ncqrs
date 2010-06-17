using System;
using FluentAssertions;
using Ncqrs.Eventing.Sourcing;
using NUnit.Framework;

namespace Ncqrs.Tests.Eventing.Sourcing
{
    [TestFixture]
    public class SourcedEventStreamTests
    {
        public class SourcedEventFoo : SourcedEvent
        {
            public SourcedEventFoo(Guid eventIdentifier, Guid eventSourceId, long eventSequence, DateTime eventTimeStamp) : base(eventIdentifier, eventSourceId, eventSequence, eventTimeStamp)
            {
            }
        }

        [Test]
        public void Appending_an_event_while_it_is_not_owned_by_any_source_should_cause_an_exception()
        {
            var target = new SourcedEventStream();
            var anSourcedEvent = new SourcedEventFoo(Guid.NewGuid(), Guid.NewGuid(), 1, DateTime.UtcNow);

            Action act = () => target.Append(anSourcedEvent);
            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void Appending_an_event_with_wrong_sequence_should_cause_an_exception()
        {
            var anEventSourceId = Guid.NewGuid();
            var wrongSequence = 999;

            var target = new SourcedEventStream(anEventSourceId);
            var anSourcedEvent = new SourcedEventFoo(Guid.NewGuid(), Guid.NewGuid(), wrongSequence, DateTime.UtcNow);

            Action act = () => target.Append(anSourcedEvent);
            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void Setting_the_event_source_id_while_there_are_already_events_in_the_stream_should_cause_exception()
        {
            var anEventSourceId = Guid.NewGuid();
            var target = new SourcedEventStream(anEventSourceId);
            var anSourcedEvent = new SourcedEventFoo(Guid.NewGuid(), anEventSourceId, 1, DateTime.UtcNow);
            
            target.Append(anSourcedEvent);

            Action act = () => target.EventSourceId = Guid.NewGuid();
            act.ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void Adding_first_event_should_cause_IsEmpty_property_to_be_false()
        {
            var anEventSourceId = Guid.NewGuid();
            var target = new SourcedEventStream(anEventSourceId);
            var anSourcedEvent = new SourcedEventFoo(Guid.NewGuid(), anEventSourceId, 1, DateTime.UtcNow);

            target.Append(anSourcedEvent);

            target.IsEmpty.Should().BeFalse();
        }
    }
}

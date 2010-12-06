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
        public void Appending_an_event_that_does_not_hold_EventSourceId_and_Sequence_should_be_accepted()
        {
            var target = new SourcedEventStream();
            var anSourcedEvent = new SourcedEventFoo(Guid.NewGuid(), UndefinedValues.UndefinedEventSourceId, UndefinedValues.UndefinedEventSequence, DateTime.UtcNow);

            Action act = () => target.Append(anSourcedEvent);
            act.ShouldNotThrow();
        }

        [Test]
        public void Appending_an_event_should_cause_the_event_owner_info_to_be_set()
        {
            var eventSourceId = Guid.NewGuid();
            var target = new SourcedEventStream(eventSourceId);
            var theSourcedEvent = new SourcedEventFoo(Guid.NewGuid(), UndefinedValues.UndefinedEventSourceId, UndefinedValues.UndefinedEventSequence, DateTime.UtcNow);

            target.Append(theSourcedEvent);

            theSourcedEvent.EventSourceId.Should().Be(eventSourceId);
            theSourcedEvent.EventSequence.Should().Be(1);
        }

        [Test]
        public void Appending_an_event_while_it_is_not_owned_by_any_source_should_cause_an_exception()
        {
            var target = new SourcedEventStream();
            var anSourcedEvent = new SourcedEventFoo(Guid.NewGuid(), Guid.NewGuid(), 1, DateTime.UtcNow);

            Action act = () => target.Append(anSourcedEvent);
            act.ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void Appending_an_event_with_sequence_should_cause_an_exception()
        {
            var anEventSourceId = Guid.NewGuid();
            var wrongSequence = 999;

            var target = new SourcedEventStream(anEventSourceId);
            var anSourcedEvent = new SourcedEventFoo(Guid.NewGuid(), UndefinedValues.UndefinedEventSourceId, wrongSequence, DateTime.UtcNow);

            Action act = () => target.Append(anSourcedEvent);
            act.ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void Setting_the_event_source_id_while_there_are_already_events_in_the_stream_should_cause_exception()
        {
            var anEventSourceId = Guid.NewGuid();
            var target = new SourcedEventStream(anEventSourceId);
            var anSourcedEvent = new SourcedEventFoo(Guid.NewGuid(), UndefinedValues.UndefinedEventSourceId, UndefinedValues.UndefinedEventSequence, DateTime.UtcNow);
            
            target.Append(anSourcedEvent);

            Action act = () => target.EventSourceId = Guid.NewGuid();
            act.ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void Adding_first_event_should_cause_IsEmpty_property_to_be_false()
        {
            var anEventSourceId = Guid.NewGuid();
            var target = new SourcedEventStream(anEventSourceId);
            var anSourcedEvent = new SourcedEventFoo(Guid.NewGuid(), UndefinedValues.UndefinedEventSourceId,
                                                     UndefinedValues.UndefinedEventSequence, DateTime.Now);

            target.Append(anSourcedEvent);

            target.IsEmpty.Should().BeFalse();
        }
    }
}

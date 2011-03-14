using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Ncqrs.Eventing;
using NUnit.Framework;
using Ncqrs.Spec;

namespace Ncqrs.Tests.Eventing
{
    [TestFixture]
    public class CommittedEventStreamTests
    {
        [Test]
        public void Init_with_source_id_only_should_set_source_id_and_have_no_events()
        {
            var sourceId = Guid.NewGuid();

            var sut = new CommittedEventStream(sourceId);
            sut.SourceId.Should().Be(sourceId);
            sut.Count().Should().Be(0);
        }

        [Test]
        public void Init_with_source_id_an_null_stream_should_set_source_id_and_have_no_events()
        {
            var sourceId = Guid.NewGuid();
            var nullStream = (IEnumerable<CommittedEvent>) null;

            var sut = new CommittedEventStream(sourceId, nullStream);
            sut.SourceId.Should().Be(sourceId);
            sut.Count().Should().Be(0);
        }

        [Test]
        public void Init_with_source_id_and_stream_should_set_source_id_and_contain_all_events_as_given()
        {
            var sourceId = Guid.NewGuid();
            var stream = new[]
            {
                new CommittedEvent(Guid.NewGuid(), Guid.NewGuid(), sourceId, 1, DateTime.Now, new object(),
                                   new Version(1, 0))
            };

            var sut = new CommittedEventStream(sourceId, stream);
            sut.SourceId.Should().Be(sourceId);

            sut.Should().BeEquivalentTo(stream);
        }

        [Test]
        public void Init_should_set_From_and_To_version_information()
        {
            var sourceId = Guid.NewGuid();
            var eventObjects = new[] { new Object(), new Object(), new Object() };
            var committedEvents = Prepare.Events(eventObjects).ForSource(sourceId, 5).ToList();

            var sut = new CommittedEventStream(sourceId, committedEvents);

            sut.FromVersion.Should().Be(committedEvents.First().EventSequence);
            sut.ToVersion.Should().Be(committedEvents.Last().EventSequence);
        }

        [Test]
        public void When_constructing_it_with_events_but_an_element_is_null_it_should_throw_ArgumentNullException()
        {
            var sourceId = Guid.NewGuid();
            var eventsWithAnNullElement = new [] { new CommittedEvent(Guid.NewGuid(), Guid.NewGuid(), sourceId, 0, DateTime.Now, new object(), new Version(1,0)), null };

            Action act = () => new CommittedEventStream(sourceId, eventsWithAnNullElement);

            act.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void When_constructing_it_with_events_where_one_has_an_incorrect_sequence_it_should_throw_argument_exception()
        {
            var sourceId = Guid.NewGuid();
            var eventObjects = new[] {new Object(), new Object(), new Object()};
            var committedEvents = Prepare.Events(eventObjects).ForSource(sourceId).ToList();

            var lastEvent = committedEvents.Last();
            const int incorrectSequence = int.MaxValue;
            var incorrectEvent = new CommittedEvent(lastEvent.CommitId, lastEvent.EventIdentifier, lastEvent.EventSourceId, incorrectSequence, lastEvent.EventTimeStamp, lastEvent.Payload, lastEvent.EventVersion);
            committedEvents[committedEvents.Count - 1] = incorrectEvent;

            Action act = () => new CommittedEventStream(sourceId, committedEvents);

            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void When_constructing_it_with_events_where_one_has_an_incorrect_event_source_id_it_should_throw_argument_exception()
        {
            var sourceId = Guid.NewGuid();
            var eventObjects = new[] { new Object(), new Object(), new Object() };
            var committedEvents = Prepare.Events(eventObjects).ForSource(sourceId).ToList();

            var lastEvent = committedEvents.Last();
            var incorrectSourceId = Guid.NewGuid();
            var incorrectEvent = new CommittedEvent(lastEvent.CommitId, lastEvent.EventIdentifier, incorrectSourceId, lastEvent.EventSequence, lastEvent.EventTimeStamp, lastEvent.Payload, lastEvent.EventVersion);
            committedEvents[committedEvents.Count - 1] = incorrectEvent;

            Action act = () => new CommittedEventStream(sourceId, committedEvents);

            act.ShouldThrow<ArgumentException>();
        }
    }
}

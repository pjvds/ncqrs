using System;
using Ncqrs.Eventing;
using NUnit.Framework;

namespace Ncqrs.Tests.Eventing
{
    [TestFixture]
    public class UncommittedEventStreamTests
    {
        [Test]
        public void When_empty_should_indicate_a_single_source()
        {
            var sut = new UncommittedEventStream(Guid.NewGuid());
            Assert.IsTrue(sut.HasSingleSource);
        }

        [Test]
        public void When_contains_single_event_should_indicate_a_single_source()
        {
            var sut = new UncommittedEventStream(Guid.NewGuid());
            sut.Append(new UncommittedEvent(Guid.NewGuid(), Guid.NewGuid(), 0, 0, DateTime.UtcNow, new object(), new Version(1,0)));
            Assert.IsTrue(sut.HasSingleSource);
        }

        [Test]
        public void When_contains_multpile_events_from_same_source_should_indicate_a_single_source()
        {
            var sut = new UncommittedEventStream(Guid.NewGuid());
            var eventSourceId = Guid.NewGuid();
            sut.Append(CreateEvent(eventSourceId));
            sut.Append(CreateEvent(eventSourceId));
            Assert.IsTrue(sut.HasSingleSource);
        }

        [Test]
        public void When_contains_multpile_events_from_different_sources_should_indicate_non_single_source()
        {
            var sut = new UncommittedEventStream(Guid.NewGuid());
            sut.Append(CreateEvent(Guid.NewGuid()));
            sut.Append(CreateEvent(Guid.NewGuid()));
            Assert.IsFalse(sut.HasSingleSource);
        }

        private static UncommittedEvent CreateEvent(Guid eventSourceId)
        {
            return new UncommittedEvent(Guid.NewGuid(), eventSourceId, 0, 0, DateTime.UtcNow, new object(), new Version(1, 0));
        }
    }
}
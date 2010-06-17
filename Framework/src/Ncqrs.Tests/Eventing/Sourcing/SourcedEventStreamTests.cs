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
    }
}

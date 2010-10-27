using System;
using System.Linq;
using Ncqrs.Eventing.Sourcing;
using NUnit.Framework;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.EventStoreTests
{
    [TestFixture]
    public class when_getting_all_events_for_an_event_source : NoDBEventStoreTestFixture
    {
        private SourcedEvent[] _returnedEvents;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _returnedEvents = EventStore.GetAllEvents(Source.EventSourceId).ToArray();
        }

        [Test]
        public void it_should_get_the_exact_same_events_that_were_committed()
        {
            Assert.That(_returnedEvents, Is.EqualTo(Events));
        }

        [Test]
        public void it_should_return_an_empty_result_for_a_non_existant_event_source()
        {
            Assert.That(EventStore.GetAllEvents(Guid.NewGuid()), Is.Empty);
        }
    }
}
using System;
using System.Linq;
using NUnit.Framework;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.EventStoreTests
{
    [TestFixture]
    public class when_getting_all_events_for_an_event_source : NoDBEventStoreTestFixture
    {
        private object [] _returnedEvents;

        [TestFixtureSetUp]
        public void SetUp()
        {
            BaseSetup();

            _returnedEvents = EventStore.ReadFrom(EventSourceId, long.MinValue, long.MaxValue).Select(x => x.Payload).ToArray();
        }

        [Test]
        public void it_should_get_the_exact_same_events_that_were_committed()
        {
            Assert.That(_returnedEvents, Is.EqualTo(Events));
        }

        [Test]
        public void it_should_return_an_empty_result_for_a_non_existant_event_source()
        {
            Assert.That(EventStore.ReadFrom(Guid.NewGuid(), long.MinValue, long.MaxValue), Is.Empty);
        }
    }
}
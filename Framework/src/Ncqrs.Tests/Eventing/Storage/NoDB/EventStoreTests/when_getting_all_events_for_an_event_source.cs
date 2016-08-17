using System;
using System.Linq;
using Xunit;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.EventStoreTests
{
    public class when_getting_all_events_for_an_event_source : NoDBEventStoreTestFixture
    {
        private object [] _returnedEvents;

        public when_getting_all_events_for_an_event_source() : base()
        {
            _returnedEvents = EventStore.ReadFrom(EventSourceId, long.MinValue, long.MaxValue).Select(x => x.Payload).ToArray();
        }

        [Fact]
        public void it_should_get_the_exact_same_events_that_were_committed()
        {
            Assert.Equal(_returnedEvents, Events);
        }

        [Fact]
        public void it_should_return_an_empty_result_for_a_non_existant_event_source()
        {
            Assert.Empty(EventStore.ReadFrom(Guid.NewGuid(), long.MinValue, long.MaxValue));
        }
    }
}
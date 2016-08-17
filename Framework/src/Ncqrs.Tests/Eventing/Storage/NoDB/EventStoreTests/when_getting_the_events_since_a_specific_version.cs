using System.Linq;
using Ncqrs.Eventing.Sourcing;
using Xunit;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.EventStoreTests
{
    public class when_getting_the_events_since_a_specific_version : NoDBEventStoreTestFixture
    {
        private object[] _returnedEvents;

        [Theory, InlineData(0), InlineData(1), InlineData(2), InlineData(3)]
        public void it_should_return_the_events_since_version(int version)
        {
            _returnedEvents = EventStore.ReadFrom(EventSourceId, version, long.MaxValue).Select(x => x.Payload).ToArray();
            for (int i = 0; i < _returnedEvents.Length; i++)
            {
                Assert.Equal(_returnedEvents[i], Events[i + version]);
            }
        }
    }
}
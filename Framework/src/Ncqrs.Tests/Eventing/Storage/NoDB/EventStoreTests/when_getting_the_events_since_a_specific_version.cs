using System.Linq;
using Ncqrs.Eventing.Sourcing;
using NUnit.Framework;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.EventStoreTests
{
    [TestFixture]
    public class when_getting_the_events_since_a_specific_version : NoDBEventStoreTestFixture
    {
        private object[] _returnedEvents;

        [TestCase(0), TestCase(1), TestCase(2), TestCase(3)]
        public void it_should_return_the_events_since_version(int version)
        {
            _returnedEvents = EventStore.ReadFrom(EventSourceId, version, long.MaxValue).Select(x => x.Payload).ToArray();
            for (int i = 0; i < _returnedEvents.Length; i++)
            {
                Assert.That(_returnedEvents[i], Is.EqualTo(Events[i + version]));
            }
        }
    }
}
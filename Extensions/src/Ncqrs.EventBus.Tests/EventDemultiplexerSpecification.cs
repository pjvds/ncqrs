using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.EventBus.Tests
{
    [TestFixture]
    public class EventDemultiplexerSpecification
    {
        [Test]
        public void When_all_queues_are_empty_event_is_fetched_from_store()
        {
            var eventStore = MockRepository.GenerateMock<IEventStore>();
            var sut = new EventDemultiplexer(eventStore);

            sut.GetNext();

            eventStore.AssertWasCalled(x => x.GetNext());
        }
    }
}
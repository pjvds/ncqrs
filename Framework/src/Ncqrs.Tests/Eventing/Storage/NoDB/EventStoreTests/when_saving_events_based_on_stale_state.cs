using NUnit.Framework;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.EventStoreTests
{
    [TestFixture]
    public class when_saving_events_based_on_stale_state : NoDBEventStoreTestFixture
    {
        [Test, ExpectedException(typeof(ConcurrencyException))]
        public void it_should_throw_a_concurrency_exception() { EventStore.Save(Source); }
    }
}
using System;
using Ncqrs.Eventing.Storage.NoDB.Tests.Fakes;
using Ncqrs.Spec;
using Xunit;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.EventStoreTests
{
    public class when_saving_events_based_on_stale_state : NoDBEventStoreTestFixture
    {
        [Fact]
        public void it_should_throw_a_concurrency_exception()
        {
            var eventStream = Prepare.Events(new AccountTitleChangedEvent("Title"))
                .ForSourceUncomitted(EventSourceId, Guid.NewGuid());

            ConcurrencyException ex = Assert.Throws<ConcurrencyException>(() => EventStore.Store(eventStream));
            Assert.NotNull(ex);

        }
    }
}
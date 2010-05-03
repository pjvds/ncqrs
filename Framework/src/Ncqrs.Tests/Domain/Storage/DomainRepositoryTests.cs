using System;
using FluentAssertions;
using Ncqrs.Eventing;
using NUnit.Framework;
using Ncqrs.Domain.Storage;
using Rhino.Mocks;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Domain;
using System.Collections.Generic;

namespace Ncqrs.Tests.Domain.Storage
{
    public class DomainRepositoryTests
    {
        public class FooEvent : DomainEvent
        { }

        public class BarEvent : DomainEvent
        { }

        public class MyAggregateRoot : AggregateRootMappedByConvention
        {
            public List<DomainEvent> AppliedEvents = new List<DomainEvent>();

            protected void OnFooEvent(FooEvent e)
            {
                AppliedEvents.Add(e);
            }

            protected void OnBarEvent(BarEvent e)
            {
                AppliedEvents.Add(e);
            }
        }

        [Test]
        public void When_a_aggregate_root_is_requested_by_id_it_should__get_the_event_from_the_store_and_load_the_aggregate_with_the_loader_with_the_events_from_the_store()
        {
            var store = MockRepository.GenerateMock<IEventStore>();
            var bus = MockRepository.GenerateMock<IEventBus>();
            var loader = MockRepository.GenerateMock<IAggregateRootLoader>();

            var repository = new DomainRepository(store, bus, loader);

            var aggId = Guid.NewGuid();
            var eventsInTheStore = new DomainEvent[] {new FooEvent(), new BarEvent()};
            var loadedAggregate = new MyAggregateRoot();

            store.Expect(s => s.GetAllEventsForEventSource(aggId)).Return(eventsInTheStore);
            loader.Expect(l => l.LoadAggregateRootFromEvents(typeof (MyAggregateRoot), eventsInTheStore)).Return(loadedAggregate);

            var result = repository.GetById<MyAggregateRoot>(aggId);

            store.VerifyAllExpectations();
            loader.VerifyAllExpectations();
            result.Should().Be(loadedAggregate);
        }
    }
}

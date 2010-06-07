using System;
using FluentAssertions;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Conversion;
using NUnit.Framework;
using Ncqrs.Domain.Storage;
using Rhino.Mocks;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Domain;
using System.Collections.Generic;
using Ncqrs.Domain.Mapping;

namespace Ncqrs.Tests.Domain.Storage
{
    public class DomainRepositoryTests
    {
        public class FooEvent : DomainEvent
        { }

        public class BarEvent : DomainEvent
        {
            public BarEvent()
            {
            }

            public BarEvent(Guid eventIdentifier, Guid aggregateRootId, long eventSequence, DateTime eventTimeStamp) : base(eventIdentifier, aggregateRootId, eventSequence, eventTimeStamp)
            {
            }
        }

        public class BarEvent_v2 : DomainEvent
        {
            public BarEvent_v2()
            {
            }

            public BarEvent_v2(Guid eventIdentifier, Guid aggregateRootId, long eventSequence, DateTime eventTimeStamp)
                : base(eventIdentifier, aggregateRootId, eventSequence, eventTimeStamp)
            {
            }
        }

        public class MyAggregateRoot : IAggregateRoot, IAggregateRootMappedWithAttributes
        {
            public void Foo()
            {
                var e = new FooEvent();
                this.ApplyEvent(e);
            }

            public void Bar()
            {
                var e = new BarEvent();
                this.ApplyEvent(e);
            }

            [EventHandlerAttribute]
            private void CatchAllHandler(DomainEvent e)
            {}
        }

        [Test]
        public void It_should_call_the_converter_for_each_event()
        {
            var store = MockRepository.GenerateMock<IEventStore>();
            var bus = MockRepository.GenerateMock<IEventBus>();
            var converter = MockRepository.GenerateMock<IEventConverter<DomainEvent, DomainEvent>>();

            var aggId = Guid.NewGuid();
            var eventsInTheStore = new DomainEvent[] { new FooEvent(), new BarEvent() };
            store.Expect(s => s.GetAllEvents(aggId)).Return(eventsInTheStore);

            var repository = new DomainRepository(store, bus, null, converter);

            repository.GetById<MyAggregateRoot>(aggId);

            converter.AssertWasCalled(c => c.Convert(null),options => options.IgnoreArguments().Repeat.Twice());
        }

        [Test]
        public void Save_test()
        {
            using (var work = NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork())
            {
                var store = MockRepository.GenerateMock<IEventStore>();
                var bus = MockRepository.GenerateMock<IEventBus>();
                var aggregate = work.Create<MyAggregateRoot>();

                aggregate.Foo();
                aggregate.Bar();

                store.Expect(s => s.Save((IEventSource)aggregate));
                bus.Expect(b => b.Publish((IEnumerable<IEvent>) null)).IgnoreArguments();

                var repository = new DomainRepository(store, bus);
                repository.Save((IEventSource)aggregate);

                bus.VerifyAllExpectations();
                store.VerifyAllExpectations();
            }
        }
    }
}

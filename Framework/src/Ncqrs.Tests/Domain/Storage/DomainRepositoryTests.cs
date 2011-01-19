using System;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Sourcing.Mapping;
using NUnit.Framework;
using Ncqrs.Domain.Storage;
using Rhino.Mocks;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Domain;
using System.Collections.Generic;

namespace Ncqrs.Tests.Domain.Storage
{
    [TestFixture]
    public class DomainRepositoryTests
    {
        public class FooEvent : SourcedEvent
        {
            public FooEvent()
            {
            }

            public FooEvent(Guid eventIdentifier, Guid aggregateRootId, long eventSequence, DateTime eventTimeStamp) : base(eventIdentifier, aggregateRootId, eventSequence, eventTimeStamp)
            {
            }
        }

        public class BarEvent : SourcedEvent
        {
            public BarEvent()
            {
            }

            public BarEvent(Guid eventIdentifier, Guid aggregateRootId, long eventSequence, DateTime eventTimeStamp) : base(eventIdentifier, aggregateRootId, eventSequence, eventTimeStamp)
            {
            }
        }

        public class BarEvent_v2 : SourcedEvent
        {
            public BarEvent_v2()
            {
            }

            public BarEvent_v2(Guid eventIdentifier, Guid aggregateRootId, long eventSequence, DateTime eventTimeStamp)
                : base(eventIdentifier, aggregateRootId, eventSequence, eventTimeStamp)
            {
            }
        }

        public class MyAggregateRoot : AggregateRootMappedWithAttributes
        {
            public void Foo()
            {
                var e = new FooEvent();
                ApplyEvent(e);
            }

            public void Bar()
            {
                var e = new BarEvent();
                ApplyEvent(e);
            }

            [EventHandler]
            private void CatchAllHandler(SourcedEvent e)
            {}
        }

        [Test]
        public void Save_test()
        {
            var commandId = Guid.NewGuid();
            using (NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork(commandId))
            {
                var store = MockRepository.GenerateMock<IEventStore>();
                var bus = MockRepository.GenerateMock<IEventBus>();
                var eventStream = new UncommittedEventStream(commandId);

                store.Expect(s => s.Store(eventStream));
                bus.Expect(b => b.Publish((IEnumerable<IPublishableEvent>) null)).IgnoreArguments();

                var repository = new DomainRepository(store, bus);
                repository.Store(eventStream);

                bus.VerifyAllExpectations();
                store.VerifyAllExpectations();
            }
        }
    }
}

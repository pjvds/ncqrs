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
        public class FooEvent
        {
        }

        public class BarEvent
        {
        }

        public class BarEvent_v2
        {
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
            private void CatchAllHandler(object e)
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

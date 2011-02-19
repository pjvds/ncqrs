using System;
using System.Collections.Generic;
using Ncqrs.Domain;
using Ncqrs.Domain.Storage;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Ncqrs.Eventing.Storage;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Tests.Domain
{
    [TestFixture]
    public class UnitOfWorkTests
    {
        [Test]
        public void Accepting_unit_of_work_stores_and_publishes_the_events()
        {
            var commandId = Guid.NewGuid();
            var store = MockRepository.GenerateMock<IEventStore>();
            var bus = MockRepository.GenerateMock<IEventBus>();
            var domainRepository = MockRepository.GenerateMock<IDomainRepository>();
            var snapshotStore = MockRepository.GenerateMock<ISnapshotStore>();
            var snapshottingPolicy = new NoSnapshottingPolicy();

            store.Expect(s => s.Store(null)).IgnoreArguments();
            bus.Expect(b => b.Publish((IEnumerable<IPublishableEvent>) null)).IgnoreArguments();

            var sut = new UnitOfWork(commandId, domainRepository, store, snapshotStore, bus, snapshottingPolicy);
            sut.Accept();

            bus.VerifyAllExpectations();
            store.VerifyAllExpectations();
        }
    }
}
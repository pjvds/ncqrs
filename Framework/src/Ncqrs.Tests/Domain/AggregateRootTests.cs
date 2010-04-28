using System;
using System.Linq;
using FluentAssertions;
using Ncqrs.Domain;
using Rhino.Mocks;
using NUnit.Framework;

namespace Ncqrs.Tests.Domain
{
    public class AggregateRootTests
    {
        public class FooEvent : DomainEvent
        {}

        public class MyAggregateRoot : AggregateRoot
        {
            public MyAggregateRoot()
            {
                RegisterHandler(new TypeThresholdedActionBasedDomainEventHandler(CatchAllEventHandler, typeof(DomainEvent), false));
            }

            public void Foo()
            {
                var e = new FooEvent();
                ApplyEvent(e);
            }

            private void CatchAllEventHandler(DomainEvent e)
            {
            }
        }

        [Test]
        public void It_should_initialize_with_a_new_id_given_by_the_generator_from_the_environment()
        {
            var generator = MockRepository.GenerateMock<IUniqueIdentifierGenerator>();
            NcqrsEnvironment.SetDefault<IUniqueIdentifierGenerator>(generator);

            var theAggregate = new MyAggregateRoot();

            generator.AssertWasCalled(g => g.GenerateNewId());
        }

        [Test]
        public void It_should_initialize_with_no_uncommited_events()
        {
            var theAggregate = new MyAggregateRoot();

            theAggregate.GetUncommitedEvents().Count().Should().Be(0);
        }

        [Test]
        public void It_should_initialize_with_version_0()
        {
            var theAggregate = new MyAggregateRoot();

            theAggregate.Version.Should().Be(0);
        }

        [Test]
        public void Applying_an_event_should_at_it_to_the_uncommited_events()
        {
            var theAggregate = new MyAggregateRoot();

            theAggregate.Foo();

            theAggregate.GetUncommitedEvents().Count().Should().Be(1);
        }

        [Test]
        public void Applying_an_event_should_not_effect_the_version()
        {
            var theAggregate = new MyAggregateRoot();
            var versionBeforeAnyOperation = theAggregate.Version;

            theAggregate.Foo();
            var versionAfterFooOperation = theAggregate.Version;

            versionAfterFooOperation.Should().Be(versionBeforeAnyOperation);
        }
    }
}

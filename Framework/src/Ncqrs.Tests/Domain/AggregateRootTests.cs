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
        public class HandledEvent : DomainEvent
        {}

        public class UnhandledEvent : DomainEvent
        {}

        public class MyAggregateRoot : AggregateRoot
        {
            public MyAggregateRoot()
            {
                RegisterHandler(new TypeThresholdedActionBasedDomainEventHandler(OnFoo, typeof(HandledEvent), false));
            }

            public void MethodThatCausesAnEventThatHasAHandler()
            {
                var e = new HandledEvent();
                ApplyEvent(e);
            }

            public void MethodThatCausesAnEventThatDoesNotConaintAHandler()
            {
                var e = new UnhandledEvent();
                ApplyEvent(e);
            }

            private void OnFoo(DomainEvent e)
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
            using (NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork())
            {
                var theAggregate = new MyAggregateRoot();

                theAggregate.MethodThatCausesAnEventThatHasAHandler();

                theAggregate.GetUncommitedEvents().Count().Should().Be(1);

                theAggregate.MethodThatCausesAnEventThatHasAHandler();

                theAggregate.GetUncommitedEvents().Count().Should().Be(2);
            }
        }

        [Test]
        public void Applying_an_event_should_not_effect_the_version()
        {
            using (NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork())
            {
                var theAggregate = new MyAggregateRoot();
                var versionBeforeAnyOperation = theAggregate.Version;

                theAggregate.MethodThatCausesAnEventThatHasAHandler();
                var versionAfterFooOperation = theAggregate.Version;

                versionAfterFooOperation.Should().Be(versionBeforeAnyOperation);
            }
        }

        [Test]
        public void Applying_an_event_when_there_is_no_unit_of_work_should_cause_an_exception()
        {
            var theAggregate = new MyAggregateRoot();
            Action act = ()=> theAggregate.MethodThatCausesAnEventThatHasAHandler();

            act.ShouldThrow<NoUnitOfWorkAvailableInThisContextException>();
        }

        [Test]
        public void Applying_an_event_with_no_handler_should_cause_an_exception()
        {
            var theAggregate = new MyAggregateRoot();
            Action act = theAggregate.MethodThatCausesAnEventThatDoesNotConaintAHandler;

            act.ShouldThrow<EventNotHandledException>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Ncqrs.Domain;
using Ncqrs.Eventing;
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
            public int FooEventHandlerInvokeCount = 0;

            public MyAggregateRoot()
            {
                RegisterHandler(new TypeThresholdedActionBasedDomainEventHandler(OnFoo, typeof(HandledEvent), false));
            }

            public MyAggregateRoot(IEnumerable<DomainEvent> history)
            {
                RegisterHandler(new TypeThresholdedActionBasedDomainEventHandler(OnFoo, typeof(HandledEvent), false));

                InitializeFromHistory(history);
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
                FooEventHandlerInvokeCount++;
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

            theAggregate.GetUncommittedEvents().Count().Should().Be(0);
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

                theAggregate.GetUncommittedEvents().Count().Should().Be(1);

                theAggregate.MethodThatCausesAnEventThatHasAHandler();

                theAggregate.GetUncommittedEvents().Count().Should().Be(2);
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

        [Test]
        public void Loading_it_from_history_should_apply_all_events()
        {
            var history = new[] {new HandledEvent(), new HandledEvent(), new HandledEvent()};

            var theAggregate = new MyAggregateRoot(history);

            theAggregate.FooEventHandlerInvokeCount.Should().Be(3);
        }

        [Test]
        public void Getting_the_uncommitted_via_the_IEventSource_interface_should_return_the_same_as_directly()
        {
            using (NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork())
            {
                var theAggregate = new MyAggregateRoot();

                theAggregate.MethodThatCausesAnEventThatHasAHandler();
                theAggregate.MethodThatCausesAnEventThatHasAHandler();

                var directResult = theAggregate.GetUncommittedEvents();
                var viaInterfaceResult = ((IEventSource)theAggregate).GetUncommittedEvents();
                directResult.Should().BeEquivalentTo(viaInterfaceResult);
            }
        }
    }
}

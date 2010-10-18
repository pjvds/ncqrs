using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Ncqrs.Domain;
using Ncqrs.Eventing.Sourcing;
using Rhino.Mocks;
using NUnit.Framework;

namespace Ncqrs.Tests.Domain
{
    [TestFixture]
    public class AggregateRootTests
    {
        public class HandledEvent : SourcedEvent
        {
            public HandledEvent()
            {
            }

            public HandledEvent(Guid eventIdentifier, Guid aggregateRootId, long eventSequence, DateTime eventTimeStamp) : base(eventIdentifier, aggregateRootId, eventSequence, eventTimeStamp)
            {
            }

            public void OverrideAggregateRootId(Guid id)
            {
                this.GetType().GetProperty("EventSourceId").SetValue(this, id, null);
            }
        }

        public class UnhandledEvent : SourcedEvent
        {}

        public class MyAggregateRoot : AggregateRoot
        {
            public int FooEventHandlerInvokeCount = 0;

            public MyAggregateRoot()
            {
                RegisterHandler(new TypeThresholdedActionBasedDomainEventHandler(OnFoo, typeof(HandledEvent), false));
            }

            public MyAggregateRoot(Guid id) : base(id)
            {
                RegisterHandler(new TypeThresholdedActionBasedDomainEventHandler(OnFoo, typeof(HandledEvent), false));
            }

            public MyAggregateRoot(IEnumerable<SourcedEvent> history)
            {
                RegisterHandler(new TypeThresholdedActionBasedDomainEventHandler(OnFoo, typeof(HandledEvent), false));

                InitializeFromHistory(history);
            }

            public new void InitializeFromHistory(IEnumerable<ISourcedEvent> history)
            {
                base.InitializeFromHistory(history);
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

            public new void ApplyEvent(ISourcedEvent e)
            {
                base.ApplyEvent(e);
            }

            private void OnFoo(ISourcedEvent e)
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
            var theAggregate = new MyAggregateRoot();

            theAggregate.MethodThatCausesAnEventThatHasAHandler();

            theAggregate.GetUncommittedEvents().Count().Should().Be(1);

            theAggregate.MethodThatCausesAnEventThatHasAHandler();

            theAggregate.GetUncommittedEvents().Count().Should().Be(2);
        }

        [Test]
        public void Applying_an_event_when_there_is_no_unit_of_work_should_not_cause_an_exception()
        {
            var theAggregate = new MyAggregateRoot();
            Action act = ()=>theAggregate.MethodThatCausesAnEventThatHasAHandler();
            act.ShouldNotThrow();
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
            var aggId = Guid.NewGuid();
            var history = new[] { new HandledEvent(Guid.NewGuid(), aggId, 1, DateTime.UtcNow), new HandledEvent(Guid.NewGuid(), aggId, 2, DateTime.UtcNow), new HandledEvent(Guid.NewGuid(), aggId, 3, DateTime.UtcNow) };

            var theAggregate = new MyAggregateRoot(history);

            theAggregate.FooEventHandlerInvokeCount.Should().Be(3);
        }

        [Test]
        public void Getting_the_uncommitted_via_the_IEventSource_interface_should_return_the_same_as_directly()
        {
            var theAggregate = new MyAggregateRoot();

            theAggregate.MethodThatCausesAnEventThatHasAHandler();
            theAggregate.MethodThatCausesAnEventThatHasAHandler();

            var directResult = theAggregate.GetUncommittedEvents();
            var viaInterfaceResult = ((IEventSource) theAggregate).GetUncommittedEvents();
            directResult.Should().BeEquivalentTo(viaInterfaceResult);
        }

        [Test]
        public void Accepting_the_changes_should_clear_the_uncommitted_events()
        {
            var theAggregate = new MyAggregateRoot();

            theAggregate.MethodThatCausesAnEventThatHasAHandler();
            theAggregate.MethodThatCausesAnEventThatHasAHandler();
            theAggregate.MethodThatCausesAnEventThatHasAHandler();
            theAggregate.MethodThatCausesAnEventThatHasAHandler();
            theAggregate.MethodThatCausesAnEventThatHasAHandler();

            theAggregate.AcceptChanges();

            theAggregate.GetUncommittedEvents().Should().BeEmpty();
        }

        [Test]
        public void Accepting_the_changes_should_set_the_initial_version_to_the_new_version()
        {
            var theAggregate = new MyAggregateRoot();

            theAggregate.MethodThatCausesAnEventThatHasAHandler();
            theAggregate.MethodThatCausesAnEventThatHasAHandler();
            theAggregate.MethodThatCausesAnEventThatHasAHandler();
            theAggregate.MethodThatCausesAnEventThatHasAHandler();
            theAggregate.MethodThatCausesAnEventThatHasAHandler();

            theAggregate.InitialVersion.Should().Be(0);

            theAggregate.AcceptChanges();

            theAggregate.InitialVersion.Should().Be(5);
        }

        [Test]
        public void Applying_an_event_should_not_effect_the_initial_version()
        {
            using (NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork())
            {
                var theAggregate = new MyAggregateRoot();

                theAggregate.InitialVersion.Should().Be(0);
                theAggregate.MethodThatCausesAnEventThatHasAHandler();
                theAggregate.InitialVersion.Should().Be(0);
                theAggregate.MethodThatCausesAnEventThatHasAHandler();
                theAggregate.InitialVersion.Should().Be(0);
                theAggregate.MethodThatCausesAnEventThatHasAHandler();
            }
        }

        [Test]
        public void Applying_an_event_should_affect_the_version()
        {
            var theAggregate = new MyAggregateRoot();

            theAggregate.Version.Should().Be(0);

            theAggregate.MethodThatCausesAnEventThatHasAHandler();
            theAggregate.Version.Should().Be(1);
            theAggregate.GetUncommittedEvents().Last().EventSequence.Should().Be(1);

            theAggregate.MethodThatCausesAnEventThatHasAHandler();
            theAggregate.Version.Should().Be(2);
            theAggregate.GetUncommittedEvents().Last().EventSequence.Should().Be(2);

            theAggregate.MethodThatCausesAnEventThatHasAHandler();
        }

        [Test]
        public void Initializing_from_history_should_throw_an_exception_when_the_history_was_null()
        {
            IEnumerable<SourcedEvent> nullHistory = null;
            var theAggregate = new MyAggregateRoot();

            Action act = () => theAggregate.InitializeFromHistory(nullHistory);

            act.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void Initializing_from_history_should_not_throw_an_exception_when_the_history_was_empty()
        {
            var theAggregate = new MyAggregateRoot();

            IEnumerable<SourcedEvent> history = new SourcedEvent[0];

            theAggregate.InitializeFromHistory(history);
        }

        [Test]
        public void Initiazling_from_wrong_history_with_wrong_sequence_should_throw_exception()
        {
            var theAggregate = new MyAggregateRoot();
            long wrongSequence = 3;

            var event1 = new HandledEvent(Guid.NewGuid(), theAggregate.EventSourceId, wrongSequence, DateTime.UtcNow);

            IEnumerable<SourcedEvent> history = new[] { event1 };

            Action act = ()=> theAggregate.InitializeFromHistory(history);
            act.ShouldThrow<InvalidOperationException>().And.Message.Should().Contain("sequence");
        }

        [Test]
        public void Initiazling_from_history_with_correct_sequence_should_not_throw_exception()
        {
            var theAggregate = new MyAggregateRoot();

            var event1 = new HandledEvent(Guid.NewGuid(), theAggregate.EventSourceId, 1, DateTime.UtcNow);
            var event2 = new HandledEvent(Guid.NewGuid(), theAggregate.EventSourceId, 2, DateTime.UtcNow);
            var event3 = new HandledEvent(Guid.NewGuid(), theAggregate.EventSourceId, 3, DateTime.UtcNow);
            var event4 = new HandledEvent(Guid.NewGuid(), theAggregate.EventSourceId, 4, DateTime.UtcNow);
            var event5 = new HandledEvent(Guid.NewGuid(), theAggregate.EventSourceId, 5, DateTime.UtcNow);

            IEnumerable<SourcedEvent> history = new[] { event1, event2, event3, event4, event5 };

            theAggregate.InitializeFromHistory(history);
        }

        [Test]
        public void Initiazling_from_wrong_history_with_wrong_sequence_should_throw_exception2()
        {
            var theAggregate = new MyAggregateRoot();
            long wrongSequence = 8;

            var event1 = new HandledEvent(Guid.NewGuid(), theAggregate.EventSourceId, 0, DateTime.UtcNow);
            var event2 = new HandledEvent(Guid.NewGuid(), theAggregate.EventSourceId, 1, DateTime.UtcNow);
            var event3 = new HandledEvent(Guid.NewGuid(), theAggregate.EventSourceId, wrongSequence, DateTime.UtcNow);

            IEnumerable<SourcedEvent> history = new[] { event1, event2, event3 };

            Action act = () => theAggregate.InitializeFromHistory(history);
            act.ShouldThrow<InvalidOperationException>().And.Message.Should().Contain("sequence");
        }

        [Test]
        public void Applying_an_event_that_is_already_owned_by_another_source_should_cause_an_exception()
        {
            var theEvent = new HandledEvent();
            theEvent.OverrideAggregateRootId(Guid.NewGuid());
            var theAggregate = new MyAggregateRoot();

            Action act = () => theAggregate.ApplyEvent(theEvent);

            act.ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void It_could_not_be_loaded_from_history_when_it_already_contains_uncommitted_events()
        {
            var theAggregate = new MyAggregateRoot();

            theAggregate.MethodThatCausesAnEventThatHasAHandler();
            theAggregate.MethodThatCausesAnEventThatHasAHandler();

            var history = new[] {new HandledEvent(), new HandledEvent()};
            Action act = () => theAggregate.InitializeFromHistory(history);

            act.ShouldThrow<InvalidOperationException>();
        }
        
        [Test]
        public void Constructing_it_with_an_id_should_set_that_to_EventSourceId_property()
        {
            var theId = Guid.NewGuid();
            var theAggregate = new MyAggregateRoot(theId);

            theAggregate.EventSourceId.Should().Be(theId);
        }
    }
}

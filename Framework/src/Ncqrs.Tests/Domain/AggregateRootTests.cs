using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Ncqrs.Domain;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Spec;
using Rhino.Mocks;
using NUnit.Framework;

namespace Ncqrs.Tests.Domain
{
    [TestFixture]
    public class AggregateRootTests
    {
        public class HandledEvent
        {
        }

        public class UnhandledEvent
        {}

        public class MyAggregateRoot : AggregateRoot
        {
            private readonly List<UncommittedEvent> _uncomittedEvents = new List<UncommittedEvent>();
            public int FooEventHandlerInvokeCount = 0;

            public MyAggregateRoot()
            {
                RegisterHandler(new TypeThresholdedActionBasedDomainEventHandler(OnFoo, typeof(HandledEvent), "", false));
            }

            public MyAggregateRoot(Guid id) : base(id)
            {
                RegisterHandler(new TypeThresholdedActionBasedDomainEventHandler(OnFoo, typeof(HandledEvent), "", false));
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

            private void OnFoo(object e)
            {
                FooEventHandlerInvokeCount++;
            }

            protected override void OnEventApplied(UncommittedEvent appliedEvent)
            {
                base.OnEventApplied(appliedEvent);
                _uncomittedEvents.Add(appliedEvent);
            }

            public IEnumerable<UncommittedEvent> GetUncommittedEvents()
            {
                return _uncomittedEvents;
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
            var stream = Prepare.Events(new HandledEvent(), new HandledEvent(), new HandledEvent()).ForSource(aggId);
            var theAggregate = new MyAggregateRoot(aggId);

            theAggregate.InitializeFromHistory(stream);

            theAggregate.FooEventHandlerInvokeCount.Should().Be(3);
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
                var theAggregate = new MyAggregateRoot();

                theAggregate.InitialVersion.Should().Be(0);
                theAggregate.MethodThatCausesAnEventThatHasAHandler();
                theAggregate.InitialVersion.Should().Be(0);
                theAggregate.MethodThatCausesAnEventThatHasAHandler();
                theAggregate.InitialVersion.Should().Be(0);
                theAggregate.MethodThatCausesAnEventThatHasAHandler();
        }

        [Test]
        public void Applying_an_event_should_affect_the_version()
        {
            var theAggregate = new MyAggregateRoot();

            theAggregate.Version.Should().Be(0);

            theAggregate.MethodThatCausesAnEventThatHasAHandler();
            theAggregate.Version.Should().Be(1);
            theAggregate.GetUncommittedEvents().Last().EventSequence.Should().Be(1);
            theAggregate.GetUncommittedEvents().Last().InitialVersionOfEventSource.Should().Be(0);

            theAggregate.MethodThatCausesAnEventThatHasAHandler();
            theAggregate.Version.Should().Be(2);
            theAggregate.GetUncommittedEvents().Last().EventSequence.Should().Be(2);
            theAggregate.GetUncommittedEvents().Last().InitialVersionOfEventSource.Should().Be(0);

            theAggregate.MethodThatCausesAnEventThatHasAHandler();
        }

        [Test]
        public void Initializing_from_history_should_throw_an_exception_when_the_history_was_null()
        {
            var theAggregate = new MyAggregateRoot();

            Action act = () => theAggregate.InitializeFromHistory(null);

            act.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void Initializing_from_history_should_not_throw_an_exception_when_the_history_was_empty()
        {
            var theAggregate = new MyAggregateRoot();

            var history = new CommittedEventStream(Guid.Empty);

            theAggregate.InitializeFromHistory(history);
        }

        [Test]
        public void Initiazling_from_wrong_history_with_wrong_sequence_should_throw_exception()
        {
            var theAggregate = new MyAggregateRoot();
            const long wrongSequence = 3;
            var stream = new CommittedEventStream(theAggregate.EventSourceId,
                new CommittedEvent(Guid.NewGuid(), Guid.NewGuid(), theAggregate.EventSourceId, wrongSequence, DateTime.UtcNow, new HandledEvent(), new Version(1, 0)));

            Action act = ()=> theAggregate.InitializeFromHistory(stream);
            act.ShouldThrow<InvalidOperationException>().And.Message.Should().Contain("sequence");
        }

        [Test]
        public void Initiazling_from_history_with_correct_sequence_should_not_throw_exception()
        {
            var theAggregate = new MyAggregateRoot();

            var stream = new CommittedEventStream(theAggregate.EventSourceId,
                new CommittedEvent(Guid.NewGuid(), Guid.NewGuid(), theAggregate.EventSourceId, 1, DateTime.UtcNow, new HandledEvent(), new Version(1, 0)),
                new CommittedEvent(Guid.NewGuid(), Guid.NewGuid(), theAggregate.EventSourceId, 2, DateTime.UtcNow, new HandledEvent(), new Version(1, 0)),
                new CommittedEvent(Guid.NewGuid(), Guid.NewGuid(), theAggregate.EventSourceId, 3, DateTime.UtcNow, new HandledEvent(), new Version(1, 0)),
                new CommittedEvent(Guid.NewGuid(), Guid.NewGuid(), theAggregate.EventSourceId, 4, DateTime.UtcNow, new HandledEvent(), new Version(1, 0)),
                new CommittedEvent(Guid.NewGuid(), Guid.NewGuid(), theAggregate.EventSourceId, 5, DateTime.UtcNow, new HandledEvent(), new Version(1, 0)));

            theAggregate.InitializeFromHistory(stream);
        }

        [Test]
        public void It_could_not_be_loaded_from_history_when_it_already_contains_uncommitted_events()
        {
            var theAggregate = new MyAggregateRoot();

            theAggregate.MethodThatCausesAnEventThatHasAHandler();
            theAggregate.MethodThatCausesAnEventThatHasAHandler();

            var stream = Prepare.Events(new HandledEvent(), new HandledEvent()).ForSource(theAggregate.EventSourceId);

            Action act = () => theAggregate.InitializeFromHistory(stream);

            act.ShouldThrow<InvalidOperationException>();
        }
        
        [Test]
        public void Constructing_it_with_an_id_should_set_that_to_EventSourceId_property()
        {
            var theId = Guid.NewGuid();
            var theAggregate = new MyAggregateRoot(theId);

            theAggregate.EventSourceId.Should().Be(theId);
        }

        [Test]
        public void Applying_an_event_should_call_the_event_handler_only_once()
        {
            var theAggregate = new MyAggregateRoot();

            theAggregate.MethodThatCausesAnEventThatHasAHandler();

            theAggregate.FooEventHandlerInvokeCount.Should().Be(1);
        }

        [Test]
        public void Applying_an_event_to_an_agg_root_with_history_should_call_the_event_handler_only_once()
        {
            var theAggregate = new MyAggregateRoot();

            Guid commandId = Guid.NewGuid();

            var event1 = new CommittedEvent(commandId, Guid.NewGuid(), theAggregate.EventSourceId, 1, DateTime.UtcNow, new HandledEvent(), new Version(1, 0));
            var event2 = new CommittedEvent(commandId, Guid.NewGuid(), theAggregate.EventSourceId, 2, DateTime.UtcNow, new HandledEvent(), new Version(1, 0));
            var event3 = new CommittedEvent(commandId, Guid.NewGuid(), theAggregate.EventSourceId, 3, DateTime.UtcNow, new HandledEvent(), new Version(1, 0));
            var event4 = new CommittedEvent(commandId, Guid.NewGuid(), theAggregate.EventSourceId, 4, DateTime.UtcNow, new HandledEvent(), new Version(1, 0));
            var event5 = new CommittedEvent(commandId, Guid.NewGuid(), theAggregate.EventSourceId, 5, DateTime.UtcNow, new HandledEvent(), new Version(1, 0));

            IEnumerable<CommittedEvent> history = new[] { event1, event2, event3, event4, event5 };

            ((IEventSource)theAggregate).InitializeFromHistory(new CommittedEventStream(theAggregate.EventSourceId, history));

            var eventHandlerCountAfterInitialization = theAggregate.FooEventHandlerInvokeCount;

            theAggregate.MethodThatCausesAnEventThatHasAHandler();

            theAggregate.FooEventHandlerInvokeCount.Should().Be(eventHandlerCountAfterInitialization + 1);
        }

        [Test]
        public void Should_be_able_to_register_RegisterThreadStaticEventAppliedCallbacks_from_parallel_threads()
        {
            Action<AggregateRoot, UncommittedEvent> callback = (x, y) => { };

            Action registerOneCallbackOnAggregateRoot = () => AggregateRoot.RegisterThreadStaticEventAppliedCallback(callback);
            Action registerTwoCallbacksOnAggregateRoot = () => 
            {
                AggregateRoot.RegisterThreadStaticEventAppliedCallback(callback);
                AggregateRoot.RegisterThreadStaticEventAppliedCallback(callback);
            };

            System.Threading.Tasks.Parallel.Invoke(
                registerOneCallbackOnAggregateRoot,
                registerTwoCallbacksOnAggregateRoot,
                registerOneCallbackOnAggregateRoot,
                registerTwoCallbacksOnAggregateRoot,
                registerOneCallbackOnAggregateRoot,
                registerTwoCallbacksOnAggregateRoot,
                registerOneCallbackOnAggregateRoot,
                registerTwoCallbacksOnAggregateRoot);
        }
    }
}

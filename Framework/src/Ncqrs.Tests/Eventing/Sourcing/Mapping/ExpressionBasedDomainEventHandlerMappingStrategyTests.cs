using System;
using FluentAssertions;
using Ncqrs.Domain;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Sourcing.Mapping;
using NUnit.Framework;
using System.Linq;

namespace Ncqrs.Tests.Eventing.Sourcing.Mapping
{
    [TestFixture]
    public class ExpressionBasedDomainEventHandlerMappingStrategyTests
    {
        public class IlligalStaticMethodTarget : AggregateRootMappedWithExpressions
        {
            public override void InitializeEventHandlers()
            {
                Map<object>().ToHandler(OnDomainEvent);
            }

            public static void OnDomainEvent(object e)
            { }
        }

        public class GoodTarget : AggregateRootMappedWithExpressions
        {
            public override void InitializeEventHandlers()
            {
                Map<PublicEvent>().ToHandler(OnPublicEvent);
                Map<ProtectedEvent>().ToHandler(OnProtectedEvent);
                Map<InternalEvent>().ToHandler(OnInternalEvent);
                Map<PrivateEvent>().ToHandler(OnPrivateEvent);
            }

            public class PublicEvent { }
            public class ProtectedEvent { }
            public class InternalEvent { }
            public class PrivateEvent { }

            public int PublicEventHandlerInvokeCount;
            public int ProtectedEventHandlerInvokeCount;
            public int InternalEventHandlerInvokeCount;
            public int PrivateEventHandlerInvokeCount;

            public void OnPublicEvent(PublicEvent e)
            {
                PublicEventHandlerInvokeCount++;
            }

            private void OnProtectedEvent(ProtectedEvent e)
            {
                ProtectedEventHandlerInvokeCount++;
            }

            private void OnInternalEvent(InternalEvent e)
            {
                InternalEventHandlerInvokeCount++;
            }

            private void OnPrivateEvent(PrivateEvent e)
            {
                PrivateEventHandlerInvokeCount++;
            }
        }

        public class MismatchOnEventTypeTarget : AggregateRootMappedWithExpressions
        {
            public override void InitializeEventHandlers()
            {
                Map<DerivedEvent>().ToHandler(OnPublicEvent);
            }

            public void OnPublicEvent(BaseEvent e)
            { }

            public class BaseEvent
            { }

            public class DerivedEvent : BaseEvent
            { }
        }

        public class EventMappedExactOnMethodWithDerivedEventTypeTarget : AggregateRootMappedWithExpressions
        {
            public override void InitializeEventHandlers()
            {
                Map<BaseEvent>().ToHandler(OnPublicEvent).MatchExact();
            }

            public class BaseEvent
            { }

            public class DerivedEvent : BaseEvent
            { }

            public void OnPublicEvent(BaseEvent e)
            { }
        }

        public class EventMappedOnMethodWithDerivedEventTypeTarget : AggregateRootMappedWithExpressions
        {
            public override void InitializeEventHandlers()
            {
                Map<BaseEvent>().ToHandler(OnPublicEvent);
            }

            public class BaseEvent
            { }

            public class DerivedEvent : BaseEvent
            { }

            public void OnPublicEvent(BaseEvent e)
            { }
        }

        [Test]
        public void It_should_throw_an_exception_when_mapped_method_is_static()
        {
            Action act = () => new IlligalStaticMethodTarget();
            act.ShouldThrow<InvalidEventHandlerMappingException>();
        }

        [Test]
        public void It_should_map_the_mapped_events()
        {
            var aggregate = new GoodTarget();
            var mapping = new ExpressionBasedEventHandlerMappingStrategy();

            var handlers = mapping.GetEventHandlers(aggregate);

            handlers.Count().Should().Be(4);
            handlers.Should().OnlyHaveUniqueItems();
        }

        [Test]
        public void It_should_create_the_correct_event_handlers()
        {
            var aggregate = new GoodTarget();
            var mapping = new ExpressionBasedEventHandlerMappingStrategy();

            var handlers = mapping.GetEventHandlers(aggregate);

            foreach (var handler in handlers)
            {
                handler.HandleEvent(new GoodTarget.PublicEvent());
                handler.HandleEvent(new GoodTarget.ProtectedEvent());
                handler.HandleEvent(new GoodTarget.InternalEvent());
                handler.HandleEvent(new GoodTarget.PrivateEvent());
            }

            aggregate.PublicEventHandlerInvokeCount.Should().Be(1);
            aggregate.ProtectedEventHandlerInvokeCount.Should().Be(1);
            aggregate.InternalEventHandlerInvokeCount.Should().Be(1);
            aggregate.PrivateEventHandlerInvokeCount.Should().Be(1);
        }

        [Test]
        public void It_should_not_handle_event_when_there_is_a_mapping_inheritance_type_mismatch()
        {
            var aggregate = new MismatchOnEventTypeTarget();
            var mapping = new ExpressionBasedEventHandlerMappingStrategy();

            var handlers = mapping.GetEventHandlers(aggregate);
            
            foreach (var handler in handlers)
                handler.HandleEvent(new MismatchOnEventTypeTarget.BaseEvent()).Should().BeFalse();
        }

        [Test]
        public void It_should_not_handle_event_when_there_needs_to_be_an_exact_match_and_event_types_are_derived()
        {
            var aggregate = new EventMappedExactOnMethodWithDerivedEventTypeTarget();
            var mapping = new ExpressionBasedEventHandlerMappingStrategy();

            var handlers = mapping.GetEventHandlers(aggregate);

            foreach (var handler in handlers)
                handler.HandleEvent(new EventMappedExactOnMethodWithDerivedEventTypeTarget.DerivedEvent()).Should().BeFalse();
        }

        [Test]
        public void It_should_handle_event_when_there_is_no_exact_match_and_event_types_are_derived()
        {
            var aggregate = new EventMappedOnMethodWithDerivedEventTypeTarget();
            var mapping = new ExpressionBasedEventHandlerMappingStrategy();

            var handlers = mapping.GetEventHandlers(aggregate);

            foreach (var handler in handlers)
                handler.HandleEvent(new EventMappedOnMethodWithDerivedEventTypeTarget.DerivedEvent()).Should().BeTrue();
        }
    }
}
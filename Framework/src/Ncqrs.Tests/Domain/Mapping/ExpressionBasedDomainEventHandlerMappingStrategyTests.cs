//using System;
//using FluentAssertions;
//using Ncqrs.Domain;
//using Ncqrs.Domain.Mapping;
//using NUnit.Framework;
//using System.Linq;

//namespace Ncqrs.Tests.Domain.Mapping
//{
//    public class ExpressionBasedDomainEventHandlerMappingStrategyTests
//    {
//        public class IlligalStaticMethodTarget : AggregateRootMappedWithExpressions
//        {
//            public override void InitializeEventHandlers()
//            {
//                Map<DomainEvent>().ToHandler(x => OnDomainEvent(x));
//            }

//            public static void OnDomainEvent(DomainEvent e)
//            { }
//        }

//        public class GoodTarget : AggregateRootMappedWithExpressions
//        {
//            public override void InitializeEventHandlers()
//            {
//                Map<PublicEvent>().ToHandler(x => OnPublicEvent(x));
//                Map<ProtectedEvent>().ToHandler(x => OnProtectedEvent(x));
//                Map<InternalEvent>().ToHandler(x => OnInternalEvent(x));
//                Map<PrivateEvent>().ToHandler(x => OnPrivateEvent(x));
//            }

//            public class PublicEvent : DomainEvent { }
//            public class ProtectedEvent : DomainEvent { }
//            public class InternalEvent : DomainEvent { }
//            public class PrivateEvent : DomainEvent { }

//            public int PublicEventHandlerInvokeCount;
//            public int ProtectedEventHandlerInvokeCount;
//            public int InternalEventHandlerInvokeCount;
//            public int PrivateEventHandlerInvokeCount;

//            public void OnPublicEvent(PublicEvent e)
//            {
//                PublicEventHandlerInvokeCount++;
//            }

//            private void OnProtectedEvent(ProtectedEvent e)
//            {
//                ProtectedEventHandlerInvokeCount++;
//            }

//            private void OnInternalEvent(InternalEvent e)
//            {
//                InternalEventHandlerInvokeCount++;
//            }

//            private void OnPrivateEvent(PrivateEvent e)
//            {
//                PrivateEventHandlerInvokeCount++;
//            }
//        }

//        public class MismatchOnEventTypeTarget : AggregateRootMappedWithExpressions
//        {
//            public override void InitializeEventHandlers()
//            {
//                Map<DerivedEvent>().ToHandler(x => OnPublicEvent(x));
//            }

//            public void OnPublicEvent(BaseEvent e)
//            { }

//            public class BaseEvent : DomainEvent
//            { }

//            public class DerivedEvent : BaseEvent
//            { }
//        }

//        public class EventMappedExactOnMethodWithDerivedEventTypeTarget : AggregateRootMappedWithExpressions
//        {
//            public override void InitializeEventHandlers()
//            {
//                Map<BaseEvent>().ToHandler(x => OnPublicEvent(x)).MatchExact();
//            }

//            public class BaseEvent : DomainEvent
//            { }

//            public class DerivedEvent : BaseEvent
//            { }

//            public void OnPublicEvent(BaseEvent e)
//            { }
//        }

//        public class EventMappedOnMethodWithDerivedEventTypeTarget : AggregateRootMappedWithExpressions
//        {
//            public override void InitializeEventHandlers()
//            {
//                Map<BaseEvent>().ToHandler(x => OnPublicEvent(x));
//            }

//            public class BaseEvent : DomainEvent
//            { }

//            public class DerivedEvent : BaseEvent
//            { }

//            public void OnPublicEvent(BaseEvent e)
//            { }
//        }

//        [Test]
//        public void It_should_throw_an_exception_when_mapped_method_is_static()
//        {
//            var aggregate = new IlligalStaticMethodTarget();
//            var mapping = new ExpressionBasedDomainEventHandlerMappingStrategy();

//            Action act = () => mapping.GetEventHandlersFromAggregateRoot(aggregate);
//            act.ShouldThrow<InvalidEventHandlerMappingException>();
//        }

//        [Test]
//        public void It_should_map_the_mapped_events()
//        {
//            var aggregate = new GoodTarget();
//            var mapping = new ExpressionBasedDomainEventHandlerMappingStrategy();

//            var handlers = mapping.GetEventHandlersFromAggregateRoot(aggregate);

//            handlers.Count().Should().Be(4);
//            handlers.Should().OnlyHaveUniqueItems();
//        }

//        [Test]
//        public void It_should_create_the_correct_event_handlers()
//        {
//            var aggregate = new GoodTarget();
//            var mapping = new ExpressionBasedDomainEventHandlerMappingStrategy();

//            var handlers = mapping.GetEventHandlersFromAggregateRoot(aggregate);

//            foreach (var handler in handlers)
//            {
//                handler.HandleEvent(new GoodTarget.PublicEvent());
//                handler.HandleEvent(new GoodTarget.ProtectedEvent());
//                handler.HandleEvent(new GoodTarget.InternalEvent());
//                handler.HandleEvent(new GoodTarget.PrivateEvent());
//            }

//            aggregate.PublicEventHandlerInvokeCount.Should().Be(1);
//            aggregate.ProtectedEventHandlerInvokeCount.Should().Be(1);
//            aggregate.InternalEventHandlerInvokeCount.Should().Be(1);
//            aggregate.PrivateEventHandlerInvokeCount.Should().Be(1);
//        }

//        [Test]
//        public void It_should_not_handle_event_when_there_is_a_mapping_inheritance_type_mismatch()
//        {
//            var aggregate = new MismatchOnEventTypeTarget();
//            var mapping = new ExpressionBasedDomainEventHandlerMappingStrategy();

//            var handlers = mapping.GetEventHandlersFromAggregateRoot(aggregate);
            
//            foreach (var handler in handlers)
//                handler.HandleEvent(new MismatchOnEventTypeTarget.BaseEvent()).Should().BeFalse();
//        }

//        [Test]
//        public void It_should_not_handle_event_when_there_needs_to_be_an_exact_match_and_event_types_are_derived()
//        {
//            var aggregate = new EventMappedExactOnMethodWithDerivedEventTypeTarget();
//            var mapping = new ExpressionBasedDomainEventHandlerMappingStrategy();

//            var handlers = mapping.GetEventHandlersFromAggregateRoot(aggregate);

//            foreach (var handler in handlers)
//                handler.HandleEvent(new EventMappedExactOnMethodWithDerivedEventTypeTarget.DerivedEvent()).Should().BeFalse();
//        }

//        [Test]
//        public void It_should_handle_event_when_there_is_no_exact_match_and_event_types_are_derived()
//        {
//            var aggregate = new EventMappedOnMethodWithDerivedEventTypeTarget();
//            var mapping = new ExpressionBasedDomainEventHandlerMappingStrategy();

//            var handlers = mapping.GetEventHandlersFromAggregateRoot(aggregate);

//            foreach (var handler in handlers)
//                handler.HandleEvent(new EventMappedOnMethodWithDerivedEventTypeTarget.DerivedEvent()).Should().BeTrue();
//        }
//    }
//}
using System;
using System.Linq;
using FluentAssertions;
using Ncqrs.Domain;
using Ncqrs.Domain.Mapping;
using NUnit.Framework;

namespace Ncqrs.Tests.Domain.Mapping
{
    public class ConventionBasedDomainEventHandlerMappingStrategyTests
    {
        public class IlligalStaticMethodTarget : AggregateRootMappedByConvention
        {
            public static void OnDomainEvent(DomainEvent e)
            { }
        }

        public class NoParameterMethodTarget : AggregateRootMappedByConvention
        {
            public void OnMyEvent()
            {
            }
        }

        public class MoreThenOneParameterMethodTarget : AggregateRootMappedByConvention
        {
            public void OnDomainEvent(DomainEvent e1, DomainEvent e2)
            {
            }
        }

        public class NotADomainEventTarget : AggregateRootMappedByConvention
        {
            public void OnDomainEvent(String e)
            {
            }
        }

        public class GoodTarget : AggregateRootMappedByConvention
        {
            public class PublicEvent : DomainEvent { }
            public class ProtectedEvent : DomainEvent { }
            public class InternalEvent : DomainEvent { }
            public class PrivateEvent : DomainEvent { }

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

        [Test]
        public void It_should_skip_when_mapped_method_is_static()
        {
            var aggregate = new IlligalStaticMethodTarget();
            var mapping = new ConventionBasedDomainEventHandlerMappingStrategy();

            var handlers = mapping.GetEventHandlersFromAggregateRoot(aggregate);

            handlers.Should().BeEmpty();
        }

        [Test]
        public void It_should_skip_when_mapped_method_does_not_have_a_parameter()
        {
            var aggregate = new NoParameterMethodTarget();
            var mapping = new ConventionBasedDomainEventHandlerMappingStrategy();

            var handlers = mapping.GetEventHandlersFromAggregateRoot(aggregate);

            handlers.Should().BeEmpty();
        }

        [Test]
        public void It_should_skip_when_mapped_method_does_have_more_then_one_parameter()
        {
            var aggregate = new MoreThenOneParameterMethodTarget();
            var mapping = new ConventionBasedDomainEventHandlerMappingStrategy();

            var handlers = mapping.GetEventHandlersFromAggregateRoot(aggregate);

            handlers.Should().BeEmpty();
        }

        [Test]
        public void It_should_skip_when_mapped_method_does_not_have_a_DomainEvent_as_parameter()
        {
            var aggregate = new NotADomainEventTarget();
            var mapping = new ConventionBasedDomainEventHandlerMappingStrategy();

            var handlers = mapping.GetEventHandlersFromAggregateRoot(aggregate);

            handlers.Should().BeEmpty();
        }

        [Test]
        public void It_should_map_the_mapped_events()
        {
            var aggregate = new GoodTarget();
            var mapping = new ConventionBasedDomainEventHandlerMappingStrategy();

            var handlers = mapping.GetEventHandlersFromAggregateRoot(aggregate);

            handlers.Count().Should().Be(4);
            handlers.Should().OnlyHaveUniqueItems();
        }

        [Test]
        public void It_should_create_the_correct_event_handlers()
        {
            var aggregate = new GoodTarget();
            var mapping = new ConventionBasedDomainEventHandlerMappingStrategy();

            var handlers = mapping.GetEventHandlersFromAggregateRoot(aggregate);

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
    }
}

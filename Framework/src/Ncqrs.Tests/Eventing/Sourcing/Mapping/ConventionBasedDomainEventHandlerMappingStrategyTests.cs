using System;
using System.Linq;
using FluentAssertions;
using Ncqrs.Domain;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Sourcing.Mapping;
using NUnit.Framework;

namespace Ncqrs.Tests.Eventing.Sourcing.Mapping
{
    [TestFixture]
    public class ConventionBasedDomainEventHandlerMappingStrategyTests
    {
        public class IlligalStaticMethodTarget : AggregateRootMappedByConvention
        {
            public static void OnDomainEvent(SourcedEvent e)
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
            public void OnDomainEvent(SourcedEvent e1, SourcedEvent e2)
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
            public class PublicEvent : SourcedEvent { }
            public class ProtectedEvent : SourcedEvent { }
            public class InternalEvent : SourcedEvent { }
            public class PrivateEvent : SourcedEvent { }

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
            var mapping = new ConventionBasedEventHandlerMappingStrategy();

            var handlers = mapping.GetEventHandlers(aggregate);

            handlers.Should().BeEmpty();
        }

        [Test]
        public void It_should_skip_when_mapped_method_does_not_have_a_parameter()
        {
            var aggregate = new NoParameterMethodTarget();
            var mapping = new ConventionBasedEventHandlerMappingStrategy();

            var handlers = mapping.GetEventHandlers(aggregate);

            handlers.Should().BeEmpty();
        }

        [Test]
        public void It_should_skip_when_mapped_method_does_have_more_then_one_parameter()
        {
            var aggregate = new MoreThenOneParameterMethodTarget();
            var mapping = new ConventionBasedEventHandlerMappingStrategy();

            var handlers = mapping.GetEventHandlers(aggregate);

            handlers.Should().BeEmpty();
        }

        [Test]
        public void It_should_skip_when_mapped_method_does_not_have_a_DomainEvent_as_parameter()
        {
            var aggregate = new NotADomainEventTarget();
            var mapping = new ConventionBasedEventHandlerMappingStrategy();

            var handlers = mapping.GetEventHandlers(aggregate);

            handlers.Should().BeEmpty();
        }

        [Test]
        public void It_should_map_the_mapped_events()
        {
            var aggregate = new GoodTarget();
            var mapping = new ConventionBasedEventHandlerMappingStrategy();

            var handlers = mapping.GetEventHandlers(aggregate);

            handlers.Count().Should().Be(4);
            handlers.Should().OnlyHaveUniqueItems();
        }

        [Test]
        public void It_should_create_the_correct_event_handlers()
        {
            var aggregate = new GoodTarget();
            var mapping = new ConventionBasedEventHandlerMappingStrategy();

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
    }
}

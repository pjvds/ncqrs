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
            public static void OnDomainEvent(object e)
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
            public void OnDomainEvent(object e1, object e2)
            {
            }
        }

        public class NotAIEventSourceTarget : AggregateRootMappedByConvention
        {
            public void OnDomainEvent(String e)
            {
            }
        }

        public class GoodTarget : AggregateRootMappedByConvention
        {
            public class PublicEvent { }
            public class ProtectedEvent { }
            public class InternalEvent { }
            public class PrivateEvent  { }

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
        public void It_should_skip_when_mapped_method_does_not_have_a_EventBaseType_as_parameter()
        {
            var aggregate = new NotAIEventSourceTarget();
            var mapping = new ConventionBasedEventHandlerMappingStrategy {EventBaseType = typeof (ISourcedEvent)};

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

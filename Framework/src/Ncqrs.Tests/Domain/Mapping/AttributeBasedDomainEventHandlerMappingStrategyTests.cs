using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Ncqrs.Domain;
using Ncqrs.Domain.Mapping;

namespace Ncqrs.Tests.Domain.Mapping
{
    public class AttributeBasedDomainEventHandlerMappingStrategyTests
    {
        public class IlligalStaticMethodTarget
            : AggregateRoot
        {
            [EventHandler]
            public static void MyEventHandlerMethod(DomainEvent e)
            {}
        }

        public class NoParameterMethodTarget : AggregateRoot
        {
            [EventHandler]
            public void MyEventHandlerMethod()
            {
            }
        }

        public class MoreThenOneParameterMethodTarget : AggregateRoot
        {
            [EventHandler]
            public void MyEventHandlerMethod(DomainEvent e1, DomainEvent e2)
            {
            }
        }

        public class NotADomainEventTarget : AggregateRoot
        {
            [EventHandler]
            public void MyEventHandlerMethod(String e)
            {
            }
        }

        public class GoodTarget : AggregateRoot
        {
            public class PublicEvent : DomainEvent { }
            public class ProtectedEvent : DomainEvent { }
            public class InternalEvent : DomainEvent { }
            public class PrivateEvent : DomainEvent { }

            [EventHandler]
            public void PublicEventHandler(PublicEvent e)
            {
            }

            [EventHandler]
            private void ProtectedEventHandler(ProtectedEvent e)
            {
            }

            [EventHandler]
            private void InternalEventHandler(InternalEvent e)
            {
            }

            [EventHandler]
            private void PrivateEventHandler(PrivateEvent e)
            {
            }
        }

        [Test]
        public void It_should_throw_an_exception_when_mapped_method_is_static()
        {
            var aggregate = new IlligalStaticMethodTarget();
            var mapping = new AttributeBasedDomainEventHandlerMappingStrategy();

            Action act = ()=>mapping.GetEventHandlersFromAggregateRoot(aggregate);

            act.ShouldThrow<InvalidEventHandlerMappingException>();
        }

        [Test]
        public void It_should_throw_an_exception_when_mapped_method_does_not_have_a_parameter()
        {
            var aggregate = new NoParameterMethodTarget();
            var mapping = new AttributeBasedDomainEventHandlerMappingStrategy();

            Action act = () => mapping.GetEventHandlersFromAggregateRoot(aggregate);

            act.ShouldThrow<InvalidEventHandlerMappingException>();
        }

        [Test]
        public void It_should_throw_an_exception_when_mapped_method_does_have_more_then_one_parameter()
        {
            var aggregate = new MoreThenOneParameterMethodTarget();
            var mapping = new AttributeBasedDomainEventHandlerMappingStrategy();

            Action act = () => mapping.GetEventHandlersFromAggregateRoot(aggregate);

            act.ShouldThrow<InvalidEventHandlerMappingException>();
        }

        [Test]
        public void It_should_throw_an_exception_when_mapped_method_does_not_have_a_DomainEvent_as_parameter()
        {
            var aggregate = new NotADomainEventTarget();
            var mapping = new AttributeBasedDomainEventHandlerMappingStrategy();

            Action act = () => mapping.GetEventHandlersFromAggregateRoot(aggregate);

            act.ShouldThrow<InvalidEventHandlerMappingException>();
        }

        [Test]
        public void It_should_map_the_mapped_events()
        {
            var aggregate = new GoodTarget();
            var mapping = new AttributeBasedDomainEventHandlerMappingStrategy();

            var handlers = mapping.GetEventHandlersFromAggregateRoot(aggregate);

            handlers.Count().Should().Be(4);
        }
    }
}

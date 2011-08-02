using System;
using System.Linq;
using FluentAssertions;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Sourcing.Mapping;
using NUnit.Framework;
using Ncqrs.Domain;

namespace Ncqrs.Tests.Eventing.Sourcing.Mapping
{
    [TestFixture]
    public class AttributeBasedDomainEventHandlerMappingStrategyTests
    {
        public class IlligalStaticMethodTarget
            : AggregateRoot
        {
            [EventHandler]
            public static void MyEventHandlerMethod(object e)
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
            public void MyEventHandlerMethod(object e1, object e2)
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
            public class PublicEvent { }
            public class ProtectedEvent { }
            public class InternalEvent { }
            public class PrivateEvent { }

            public int PublicEventHandlerInvokeCount;
            public int ProtectedEventHandlerInvokeCount;
            public int InternalEventHandlerInvokeCount;
            public int PrivateEventHandlerInvokeCount;
            public int CatchAllEventHandlerInvokeCount;

            [EventHandler]
            public void PublicEventHandler(PublicEvent e)
            {
                PublicEventHandlerInvokeCount++;
            }

            [EventHandler]
            private void ProtectedEventHandler(ProtectedEvent e)
            {
                ProtectedEventHandlerInvokeCount++;
            }

            [EventHandler]
            private void InternalEventHandler(InternalEvent e)
            {
                InternalEventHandlerInvokeCount++;
            }

            [EventHandler]
            private void PrivateEventHandler(PrivateEvent e)
            {
                PrivateEventHandlerInvokeCount++;
            }

            [EventHandler]
            private  void CatchAllEventHandler(object e)
            {
                CatchAllEventHandlerInvokeCount++;
            }
        }

        [Test]
        public void It_should_throw_an_exception_when_mapped_method_is_static()
        {
            var aggregate = new IlligalStaticMethodTarget();
            var mapping = new AttributeBasedEventHandlerMappingStrategy();

            Action act = () => mapping.GetEventHandlers(aggregate);

            act.ShouldThrow<InvalidEventHandlerMappingException>();
        }

        [Test]
        public void It_should_throw_an_exception_when_mapped_method_does_not_have_a_parameter()
        {
            var aggregate = new NoParameterMethodTarget();
            var mapping = new AttributeBasedEventHandlerMappingStrategy();

            Action act = () => mapping.GetEventHandlers(aggregate);

            act.ShouldThrow<InvalidEventHandlerMappingException>();
        }

        [Test]
        public void It_should_throw_an_exception_when_mapped_method_does_have_more_then_one_parameter()
        {
            var aggregate = new MoreThenOneParameterMethodTarget();
            var mapping = new AttributeBasedEventHandlerMappingStrategy();

            Action act = () => mapping.GetEventHandlers(aggregate);

            act.ShouldThrow<InvalidEventHandlerMappingException>();
        }

        [Test]
        public void It_should_map_the_mapped_events()
        {
            var aggregate = new GoodTarget();
            var mapping = new AttributeBasedEventHandlerMappingStrategy();

            var handlers = mapping.GetEventHandlers(aggregate);

            handlers.Count().Should().Be(5);
        }

        [Test]
        public void It_should_create_the_correct_event_handlers()
        {
            var aggregate = new GoodTarget();
            var mapping = new AttributeBasedEventHandlerMappingStrategy();

            var handlers = mapping.GetEventHandlers(aggregate);

            foreach(var handler in handlers)
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
            aggregate.CatchAllEventHandlerInvokeCount.Should().Be(4);
        }
    }
}

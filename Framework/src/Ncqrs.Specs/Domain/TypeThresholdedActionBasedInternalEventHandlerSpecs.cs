using System;
using Ncqrs.Domain;
using Ncqrs.Eventing;
using NUnit.Framework;
using FluentAssertions;

namespace Ncqrs.Specs.Domain
{
    [TestFixture]
    public class TypeThresholdedActionBasedInternalEventHandlerSpecs
    {
        public class FooEvent : EventBase
        {
        }

        public class BarEvent : FooEvent
        {
        }

        [Test]
        public void When_a_new_instance_is_initialized_with_a_type_that_is_not_of_an_event_it_should_throw_an_exception()
        {
            var wrongEventType = typeof (String);
            Action<IEvent> action = (e) => e.ToString();

            Action creatingNewEventHandlerWithWrongEventType = () => new TypeThresholdedActionBasedInternalEventHandler(action, wrongEventType);

            creatingNewEventHandlerWithWrongEventType.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void Threshold_should_hold_event_when_it_is_of_a_higher_type_when_exact_is_true()
        {
            Boolean handlerActionWasCalled = false;
            Action<IEvent> handlerAction = (e) => handlerActionWasCalled = true;

            var handler = new TypeThresholdedActionBasedInternalEventHandler(handlerAction, typeof (FooEvent), true);
            var handeled = handler.HandleEvent(new BarEvent());

            handeled.Should().Be(false);
            handlerActionWasCalled.Should().Be(false);
        }

        [Test]
        public void Threshold_should_not_hold_event_when_it_is_of_a_higher_type_when_exact_is_false()
        {
            Boolean handlerActionWasCalled = false;
            Action<IEvent> handlerAction = (e) => handlerActionWasCalled = true;

            var handler = new TypeThresholdedActionBasedInternalEventHandler(handlerAction, typeof(FooEvent), false);
            var handeled = handler.HandleEvent(new BarEvent());

            handeled.Should().Be(true);
            handlerActionWasCalled.Should().Be(true);
        }

        [Test]
        public void Threshold_should_hold_event_when_it_is_of_the_same_type_when_exact_is_true()
        {
            Boolean handlerActionWasCalled = false;
            Action<IEvent> handlerAction = (e) => handlerActionWasCalled = true;

            var handler = new TypeThresholdedActionBasedInternalEventHandler(handlerAction, typeof(FooEvent), true);
            var handeled = handler.HandleEvent(new FooEvent());

            handeled.Should().Be(true);
            handlerActionWasCalled.Should().Be(true);
        }
        
        [Test]
        public void Threshold_should_hold_event_when_it_is_of_a_lower_type_when_exact_is_true()
        {
            Boolean handlerActionWasCalled = false;
            Action<IEvent> handlerAction = (e) => handlerActionWasCalled = true;

            var handler = new TypeThresholdedActionBasedInternalEventHandler(handlerAction, typeof(BarEvent), true);
            var handeled = handler.HandleEvent(new FooEvent());

            handeled.Should().Be(false);
            handlerActionWasCalled.Should().Be(false);
        }

        [Test]
        public void Threshold_should_hold_event_when_it_is_of_a_lower_type_when_exact_is_false()
        {
            Boolean handlerActionWasCalled = false;
            Action<IEvent> handlerAction = (e) => handlerActionWasCalled = true;

            var handler = new TypeThresholdedActionBasedInternalEventHandler(handlerAction, typeof(BarEvent), false);
            var handeled = handler.HandleEvent(new FooEvent());

            handeled.Should().Be(false);
            handlerActionWasCalled.Should().Be(false);
        }
    }
}

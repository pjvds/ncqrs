using System;
using FluentAssertions;
using Ncqrs.Eventing.Sourcing;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Tests.Domain
{
    public class SourcedEventHanderTests
    {
        public class FooDomainEvent : SourcedEvent
        {}

        [Test]
        public void Invoking_it_with_a_lower_type_should_make_the_HandleEvent_method_return_false()
        {
            var theHandler = MockRepository.GenerateMock<SourcedEventHander<FooDomainEvent>>();
            var aDomainEvent = MockRepository.GenerateMock<SourcedEvent>();

            var handlerAsIDomainEventHandler = theHandler as ISourcedEventHandler;
            var handled = handlerAsIDomainEventHandler.HandleEvent(aDomainEvent);

            handled.Should().Be(false);
        }

        [Test]
        public void Invoking_it_with_a_lower_type_should_not_call_the_implemented_HandleEvent_method()
        {
            var theHandler = MockRepository.GenerateMock<SourcedEventHander<FooDomainEvent>>();
            var aDomainEvent = MockRepository.GenerateMock<SourcedEvent>();

            var handlerAsIDomainEventHandler = theHandler as ISourcedEventHandler;
            handlerAsIDomainEventHandler.HandleEvent(aDomainEvent);

            theHandler.AssertWasNotCalled(h=>h.HandleEvent(null), options=>options.IgnoreArguments());
        }

        [Test]
        public void Invoking_it_a_type_that_is_the_same_as_the_TEventType_should_invoke_the_handle_event_method()
        {
            var theHandler = MockRepository.GenerateMock<SourcedEventHander<FooDomainEvent>>();
            var theFooEvent = new FooDomainEvent();

            var handlerAsIDomainEventHandler = theHandler as ISourcedEventHandler;
            handlerAsIDomainEventHandler.HandleEvent(theFooEvent);

            theHandler.AssertWasCalled(h => h.HandleEvent(theFooEvent));
        }
    }
}

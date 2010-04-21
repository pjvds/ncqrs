using System;
using FluentAssertions;
using Ncqrs.Domain.Mapping;
using NUnit.Framework;

namespace Ncqrs.Domain.Specs.Mapping
{
    [TestFixture]
    public class ConventionBasedMappingStrategySpecs
    {
        public class FooEvent : IEvent
        {}

        public class BarEvent : IEvent
        {}

        public class TwoEventHandlerWithTheSameEventObject : AggregateRoot
        {
            public void OnFooEvent(FooEvent evnt)
            {}

            public void onFooEvent(FooEvent evnt)
            {}
        }

        [Test]
        public void When_calling_GetEventHandlersFromAggregateRoot_with_a_object_that_contains_two_event_handlers_that_handle_the_same_event_type_it_shoud_throw_an_exception()
        {
            var strategy = new ConventionBasedMappingStrategy();
            var target = new TwoEventHandlerWithTheSameEventObject();

            Action act = () => strategy.GetEventHandlersFromAggregateRoot(target);

            act.ShouldThrow<Exception>();
        }
    }
}

using System;
using Ncqrs.Eventing;
using NUnit.Framework;
using Ncqrs.Eventing.ServiceModel.Bus;
using Rhino.Mocks;
using Ncqrs.Domain;

namespace Ncqrs.Specs.Eventing.ServiceModel.Bus
{
    [TestFixture]
    public class InProcessEventBusSpecs
    {
        public class ADomainEvent : DomainEvent
        {
            
        }

        public class AEvent : EventBase
        {
        }

        [Test]
        public void When_a_catch_all_handler_is_register_it_should_be_called_for_all_events()
        {
            var catchAllEventHandler = MockRepository.GenerateStrictMock<IEventHandler>();

            var bus = new InProcessEventBus();
            bus.RegisterHandler(catchAllEventHandler);

            bus.Publish(new ADomainEvent());
            bus.Publish(new AEvent());

            catchAllEventHandler.AssertWasCalled(h=>h.Handle(null), options => options.Repeat.Twice());
        }

        [Test]
        public void When_a_handler_is_registered_for_a_specific_type_it_should_not_receive_other_events()
        {
            var aDomainEventEventHandler = MockRepository.GenerateMock<IEventHandler>();

            var bus = new InProcessEventBus();
            bus.RegisterHandler(typeof(ADomainEvent), aDomainEventEventHandler);

            bus.Publish(new ADomainEvent());
            bus.Publish(new AEvent());
            bus.Publish(new AEvent());
            bus.Publish(new ADomainEvent());
            bus.Publish(new AEvent());
            bus.Publish(new AEvent());
            bus.Publish(new AEvent());

            aDomainEventEventHandler.AssertWasCalled(h => h.Handle(null), options => options.IgnoreArguments().Repeat.Twice());
        }

        [Test]
        public void When_a_multiple_catch_all_handler_are_registered_for_they_should_all_been_called()
        {
            var catchAllEventHandler1 = MockRepository.GenerateMock<IEventHandler>();
            var catchAllEventHandler2 = MockRepository.GenerateMock<IEventHandler>();
            var catchAllEventHandler3 = MockRepository.GenerateMock<IEventHandler>();

            var bus = new InProcessEventBus();
            bus.RegisterHandler(catchAllEventHandler1);
            bus.RegisterHandler(catchAllEventHandler2);
            bus.RegisterHandler(catchAllEventHandler3);

            bus.Publish(new ADomainEvent());
            bus.Publish(new AEvent());
            bus.Publish(new AEvent());
            bus.Publish(new ADomainEvent());
            bus.Publish(new AEvent());
            bus.Publish(new AEvent());
            bus.Publish(new AEvent());

            catchAllEventHandler1.AssertWasCalled(h => h.Handle(null), options => options.IgnoreArguments().Repeat.Times(7));
            catchAllEventHandler2.AssertWasCalled(h => h.Handle(null), options => options.IgnoreArguments().Repeat.Times(7));
            catchAllEventHandler3.AssertWasCalled(h => h.Handle(null), options => options.IgnoreArguments().Repeat.Times(7));
        }

        [Test]
        public void When_a_multiple_specific_handlers_are_register_they_all_should_be_called_when_the_specific_event_is_published()
        {
            var specificEventHandler1 = MockRepository.GenerateMock<IEventHandler>();
            var specificEventHandler2 = MockRepository.GenerateMock<IEventHandler>();
            var specificEventHandler3 = MockRepository.GenerateMock<IEventHandler>();

            var bus = new InProcessEventBus();
            bus.RegisterHandler(typeof(ADomainEvent), specificEventHandler1);
            bus.RegisterHandler(typeof(ADomainEvent), specificEventHandler2);
            bus.RegisterHandler(typeof(ADomainEvent), specificEventHandler3);

            bus.Publish(new ADomainEvent());
            bus.Publish(new AEvent());
            bus.Publish(new AEvent());
            bus.Publish(new ADomainEvent());
            bus.Publish(new AEvent());
            bus.Publish(new AEvent());
            bus.Publish(new AEvent());

            specificEventHandler1.AssertWasCalled(h => h.Handle(null), options => options.IgnoreArguments().Repeat.Times(2));
            specificEventHandler2.AssertWasCalled(h => h.Handle(null), options => options.IgnoreArguments().Repeat.Times(2));
            specificEventHandler3.AssertWasCalled(h => h.Handle(null), options => options.IgnoreArguments().Repeat.Times(2));
        }
    }
}

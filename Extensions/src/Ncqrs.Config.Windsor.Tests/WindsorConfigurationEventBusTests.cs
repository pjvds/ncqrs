using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Sourcing;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Config.Windsor.Tests
{
    public class WindsorConfigurationEventBusTests
    {
        WindsorContainer _container;
        FakeEvent _testEvent;
        IEventHandler<FakeEvent> _handler1;
        IEventHandler<SourcedEvent> _handler2;
        IEventHandler<IEvent> _handler3;

        [SetUp]
        public void SetUp()
        {
            _testEvent = new FakeEvent();
            _handler1 = MockRepository.GenerateMock<IEventHandler<FakeEvent>>();
            _handler2 = MockRepository.GenerateMock<IEventHandler<SourcedEvent>>();
            _handler3 = MockRepository.GenerateMock<IEventHandler<IEvent>>();
            _container = new WindsorContainer();
            _container.Register(
                Component.For<IWindsorContainer>().Instance(_container),
                Component.For<IEventHandler<FakeEvent>>().Instance(_handler1),
                Component.For<IEventHandler<SourcedEvent>>().Instance(_handler2),
                Component.For<IEventHandler<IEvent>>().Instance(_handler3),
                Component.For<IEventBus>().ImplementedBy<WindsorInProcessEventBus>());
            var svc = _container.Resolve<IEventBus>();
            svc.Publish(_testEvent);
        }
        
        [Test]
        public void it_should_call_the_class_handler() { _handler1.AssertWasCalled(x => x.Handle(_testEvent)); }

        [Test]
        public void it_should_call_the_base_class_handler() { _handler2.AssertWasCalled(x => x.Handle(_testEvent)); }
        
        [Test]
        public void it_should_call_the_interface_handler() { _handler3.AssertWasCalled(x => x.Handle(_testEvent)); }
    }

    public class FakeEvent : SourcedEvent {}
}
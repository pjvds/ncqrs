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
        UncommittedEvent _testEvent;
        IEventHandler<FakeEvent> _handler1;
        IEventHandler<FakeEventBase> _handler2;
        IEventHandler<IFakeEventInterface> _handler3;

        [SetUp]
        public void SetUp()
        {
            _testEvent = new UncommittedEvent(Guid.NewGuid(), Guid.NewGuid(), 1, 1, DateTime.UtcNow, new FakeEvent(),
                                              new Version(1, 0));
            _handler1 = MockRepository.GenerateMock<IEventHandler<FakeEvent>>();
            _handler2 = MockRepository.GenerateMock<IEventHandler<FakeEventBase>>();
            _handler3 = MockRepository.GenerateMock<IEventHandler<IFakeEventInterface>>();
            _container = new WindsorContainer();
            _container.Register(
                Component.For<IWindsorContainer>().Instance(_container),
                Component.For<IEventHandler<FakeEvent>>().Instance(_handler1),
                Component.For<IEventHandler<FakeEventBase>>().Instance(_handler2),
                Component.For<IEventHandler<IFakeEventInterface>>().Instance(_handler3),
                Component.For<IEventBus>().ImplementedBy<WindsorInProcessEventBus>());
            var svc = _container.Resolve<IEventBus>();
            svc.Publish(_testEvent);
        }
        
        [Test]
        public void it_should_call_the_class_handler_once() { _handler1.AssertWasCalled(x => x.Handle(null), o => o.Repeat.Once().IgnoreArguments()); }

        [Test]
        public void it_should_call_the_base_class_handler_once() { _handler2.AssertWasCalled(x => x.Handle(null), o => o.Repeat.Once().IgnoreArguments()); }
        
        [Test]
        public void it_should_call_the_interface_handler_once() { _handler3.AssertWasCalled(x => x.Handle(null), o => o.Repeat.Once().IgnoreArguments()); }
    }

    public class FakeEvent : FakeEventBase, IFakeEventInterface { }
    public class FakeEventBase { }
    public interface IFakeEventInterface { }
}
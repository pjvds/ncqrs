using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Sourcing;
using NUnit.Framework;
using Moq;

namespace Ncqrs.Config.Windsor.Tests
{
    [Ignore("Error with mstest adapter")]

    public class WindsorConfigurationEventBusTests
    {
        WindsorContainer _container;
        UncommittedEvent _testEvent;
        Mock<IEventHandler<FakeEvent>> _handler1;
        Mock<IEventHandler<FakeEventBase>> _handler2;
        Mock<IEventHandler<IFakeEventInterface>> _handler3;

        [SetUp]
        public void SetUp()
        {
            _testEvent = new UncommittedEvent(Guid.NewGuid(), Guid.NewGuid(), 1, 1, DateTime.UtcNow, new FakeEvent(),
                                              new Version(1, 0));
            _handler1 = new Mock<IEventHandler<FakeEvent>>();
            _handler1.SetupAllProperties();
            _handler2 = new Mock<IEventHandler<FakeEventBase>>();
            _handler2.SetupAllProperties();
            _handler3 = new Mock<IEventHandler<IFakeEventInterface>>();
            _handler3.SetupAllProperties();
            _container = new WindsorContainer();
            _container.Register(
                Component.For<IWindsorContainer>().Instance(_container),
                Component.For<IEventHandler<FakeEvent>>().Instance(_handler1.Object),
                Component.For<IEventHandler<FakeEventBase>>().Instance(_handler2.Object),
                Component.For<IEventHandler<IFakeEventInterface>>().Instance(_handler3.Object),
                Component.For<IEventBus>().ImplementedBy<WindsorInProcessEventBus>());
            var svc = _container.Resolve<IEventBus>();
            svc.Publish(_testEvent);
        }

        [Test]
        public void it_should_call_the_class_handler_once() { _handler1.Verify(x => x.Handle(null)); }

        [Test]
        public void it_should_call_the_base_class_handler_once() { _handler2.Verify(x => x.Handle(null)); }
        
        [Test]
        public void it_should_call_the_interface_handler_once() { _handler3.Verify(x => x.Handle(null)); }
    }

    public class FakeEvent : FakeEventBase, IFakeEventInterface { }
    public class FakeEventBase { }
    public interface IFakeEventInterface { }
}
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Commanding.ServiceModel;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Config.Windsor.Tests
{
    [TestFixture]
    public class when_using_windsor_to_registor_command_interceptors_and_handlers
    {
        WindsorContainer _container;
        FakeInterceptor _interceptor;
        ICommandExecutor<FakeCommand> _handler;
        FakeCommand _testCommand;
        FakeInterceptor2 _interceptor2;

        [SetUp]
        public void SetUp()
        { 
            _handler = MockRepository.GenerateStub<ICommandExecutor<FakeCommand>>();
            _container = new WindsorContainer();
            _interceptor = new FakeInterceptor();
            _interceptor2 = new FakeInterceptor2();
            _container.Register(
                Component.For<IWindsorContainer>().Instance(_container),
                Component.For<ICommandExecutor<FakeCommand>>().Instance(_handler),
                Component.For<ICommandServiceInterceptor>().Instance(_interceptor),
                Component.For<ICommandServiceInterceptor>().Instance(_interceptor2),
                Component.For<ICommandService>().ImplementedBy<WindsorCommandService>());
            var svc = _container.Resolve<ICommandService>();
            _testCommand = new FakeCommand();
            svc.Execute(_testCommand);
        }
        
        [Test]
        public void it_should_call_the_handler()
        {
            _handler.AssertWasCalled(h => h.Execute(_testCommand));
        }

        [Test]
        public void it_should_call_both_interceptors() { 
            Assert.That(_interceptor.OnBeforeExecutorResolvingCalled);
            Assert.That(_interceptor.OnBeforeExecutionCalled);
            Assert.That(_interceptor.OnAfterExecutionCalled);
            Assert.That(_interceptor2.OnBeforeExecutorResolvingCalled);
            Assert.That(_interceptor2.OnBeforeExecutionCalled);
            Assert.That(_interceptor2.OnAfterExecutionCalled);
        }

        [Test]
        public void CanExecuteCommandRepeatedly()
        { 
            var svc = _container.Resolve<ICommandService>();
            svc.Execute(new FakeCommand());
            svc.Execute(new FakeCommand());
            svc.Execute(new FakeCommand());
        }
    }

    public class FakeInterceptor2 : FakeInterceptor {}
    public class FakeInterceptor : ICommandServiceInterceptor
    {
        public bool OnBeforeExecutorResolvingCalled;
        public bool OnBeforeExecutionCalled;
        public bool OnAfterExecutionCalled;

        public void OnBeforeBeforeExecutorResolving(CommandContext context)
        {
            OnBeforeExecutorResolvingCalled = true;
            Assert.That(!context.ExecutorResolved);
            Assert.That(!context.ExecutorHasBeenCalled);
            Assert.That(context.Exception, Is.Null);
        }
        public void OnBeforeExecution(CommandContext context)
        {
            OnBeforeExecutionCalled = true;
            Assert.That(context.ExecutorResolved);
            Assert.That(!context.ExecutorHasBeenCalled);
            Assert.That(context.Exception, Is.Null);
        }
        public void OnAfterExecution(CommandContext context)
        { 
            OnAfterExecutionCalled = true;
            Assert.That(context.ExecutorResolved);
            Assert.That(context.ExecutorHasBeenCalled);
            Assert.That(context.Exception, Is.Null);
        }
    }

    public class FakeCommand : CommandBase {}
}
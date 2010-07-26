using System;
using FluentAssertions;
using Rhino.Mocks;
using Ncqrs.Commanding.ServiceModel;
using NUnit.Framework;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Commanding;

namespace Ncqrs.Tests.Commanding.ServiceModel
{
    [TestFixture]
    public class CommandServiceBaseTests
    {
        public class CommandWithExecutor : CommandBase
        {}

        public class CommandWithoutExecutor : CommandBase
        {}

        public class CommandWithExecutorThatThrowsException : CommandBase
        {}

        private ICommandService TheService
        {
            get; set;
        }

        private ICommandExecutor<CommandWithExecutor> ExecutorForCommandWithExecutor
        { get; set;
        }

        private ICommandExecutor<CommandWithExecutorThatThrowsException> ExecutorForCommandWithExecutorThatThrowsException
        { get; set; }

        private ICommandServiceInterceptor Interceptor1
        { get; set;
        }

        private ICommandServiceInterceptor Interceptor2
        { get; set;
        }

        [SetUp]
        public void Setup()
        {
            var service = new CommandService();
            ExecutorForCommandWithExecutor = MockRepository.GenerateMock<ICommandExecutor<CommandWithExecutor>>();
            ExecutorForCommandWithExecutorThatThrowsException = MockRepository.GenerateMock<ICommandExecutor<CommandWithExecutorThatThrowsException>>();

            Interceptor1 = MockRepository.GenerateMock<ICommandServiceInterceptor>();
            Interceptor2 = MockRepository.GenerateMock<ICommandServiceInterceptor>();

            ExecutorForCommandWithExecutorThatThrowsException.Stub(e=>e.Execute(null)).IgnoreArguments().Throw(new Exception());

            service.RegisterExecutor(ExecutorForCommandWithExecutor);
            service.RegisterExecutor(ExecutorForCommandWithExecutorThatThrowsException);

            service.AddInterceptor(Interceptor1);
            service.AddInterceptor(Interceptor2);

            TheService = service;
        }

        [Test]
        public void All_interceptors_should_be_called_before_execution()
        {
            Interceptor1.Replay();
            Interceptor2.Replay();
            TheService.Execute(new CommandWithExecutor());

            Interceptor1.AssertWasCalled(i => i.OnBeforeExecution(null), options => options.IgnoreArguments());
            Interceptor2.AssertWasCalled(i => i.OnBeforeExecution(null), options => options.IgnoreArguments());
        }

        [Test]
        public void All_interceptors_should_be_called_after_execution()
        {
            Interceptor1.Replay();
            Interceptor2.Replay();
            TheService.Execute(new CommandWithExecutor());

            Interceptor1.AssertWasCalled(i => i.OnAfterExecution(null), options => options.IgnoreArguments());
            Interceptor2.AssertWasCalled(i => i.OnAfterExecution(null), options => options.IgnoreArguments());
        }

        [Test]
        public void Executing_command_with_no_handler_should_cause_exception()
        {
            Action act = () => TheService.Execute(new CommandWithoutExecutor());
            act.ShouldThrow<ExecutorForCommandNotFoundException>();
        }

        [Test]
        public void Executing_command_should_executure_correct_handler_with_it()
        {
            var theCommand = new CommandWithExecutor();

            ExecutorForCommandWithExecutor.Replay();
            TheService.Execute(theCommand);

            ExecutorForCommandWithExecutor.AssertWasCalled(e => e.Execute(theCommand));
        }
    }
}

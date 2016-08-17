using System;
using FluentAssertions;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Domain;
using Xunit;
using Ncqrs.Commanding;
using Rhino.Mocks;

namespace Ncqrs.Tests.Commanding
{
    
    public class CommandExecutorBaseTests
    {
        public class FooCommand : ICommand
        {
            public Guid CommandIdentifier { get; set; }

            public long? KnownVersion { get; set; }

            public FooCommand()
            {
                CommandIdentifier = Guid.NewGuid();
            }
        }

        public class FooCommandExecutor : CommandExecutorBase<FooCommand>
        {
            public IUnitOfWorkContext LastGivenContext { get; private set;}

            public FooCommand LastGivenCommand { get; private set; }

            public FooCommandExecutor()
            {
            }

            public FooCommandExecutor(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory)
            {
            }

            protected override void ExecuteInContext(IUnitOfWorkContext context, FooCommand command)
            {
                LastGivenContext = context;
                LastGivenCommand = command;
            }
        }

        [Fact]
        public void Executing_one_with_a_custom_factory_should_give_context_created_with_that_factory()
        {
            var factory = MockRepository.GenerateMock<IUnitOfWorkFactory>();

            var aCommand = new FooCommand()
                               {
                                   CommandIdentifier = Guid.NewGuid()
                               };
            var executor = new FooCommandExecutor(factory);
            executor.Execute(aCommand);

            factory.AssertWasCalled(f => f.CreateUnitOfWork(aCommand.CommandIdentifier));
        }

        [Fact]
        public void Executing_should_call_ExecuteInContext_with_context_from_factory()
        {            
            var context = MockRepository.GenerateMock<IUnitOfWorkContext>();
            var factory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            factory.Stub(f => f.CreateUnitOfWork(Guid.NewGuid())).Return(context).IgnoreArguments();

            var aCommand = new FooCommand();
            var executor = new FooCommandExecutor(factory);
            executor.Execute(aCommand);

            executor.LastGivenContext.Should().Be(context);
        }

        [Fact]
        public void Executing_should_call_ExecuteInContext_with_given_command()
        {
            var theCommand = new FooCommand();
            var factory = MockRepository.GenerateMock<IUnitOfWorkFactory>();

            var executor = new FooCommandExecutor(factory);
            executor.Execute(theCommand);

            executor.LastGivenCommand.Should().Be(theCommand);
        }
    }
}

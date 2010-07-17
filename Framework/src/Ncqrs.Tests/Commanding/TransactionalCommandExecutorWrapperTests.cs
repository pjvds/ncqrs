using System;
using Ncqrs.Commanding;
using Rhino.Mocks;
using Ncqrs.Commanding.CommandExecution;
using NUnit.Framework;

namespace Ncqrs.Tests.Commanding
{
    [TestFixture]
    public class TransactionalCommandExecutorWrapperTests
    {
        [Test]
        public void Executing_a_command_with_it_should_call_the_executor_that_was_set_at_construct()
        {
            var aExecutor = MockRepository.GenerateMock<ICommandExecutor<ICommand>>();

            var aCommand = MockRepository.GenerateMock<ICommand>();
            var theWrapper = new TransactionalCommandExecutorWrapper<ICommand>(aExecutor);

            theWrapper.Execute(aCommand);

            aExecutor.AssertWasCalled(e=>e.Execute(aCommand));
        }
    }
}

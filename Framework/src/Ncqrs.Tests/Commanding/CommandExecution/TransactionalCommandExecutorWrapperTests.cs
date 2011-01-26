using System;
using FluentAssertions;
using Ncqrs.Commanding.CommandExecution;
using Rhino.Mocks;
using Ncqrs.Commanding;
using System.Transactions;
using NUnit.Framework;

namespace Ncqrs.Tests.Commanding.CommandExecution
{
    [TestFixture]
    public class TransactionalCommandExecutorWrapperTests
    {
        public class DummyCommand : CommandBase
        {
        }        

        [Test]
        public void When_executing_it_it_should_call_the_executor_given_via_ctor()
        {
            var theCommand = new DummyCommand();
            var theExecutor = MockRepository.GenerateMock<ICommandExecutor<DummyCommand>>();
            var theWrapper = new TransactionalCommandExecutorWrapper<DummyCommand>(theExecutor);

            theWrapper.Execute(theCommand);

            theExecutor.AssertWasCalled((e) => e.Execute(theCommand));
        }
    }
}

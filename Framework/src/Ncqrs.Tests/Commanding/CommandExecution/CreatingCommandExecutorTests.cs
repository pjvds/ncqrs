using System;
using FluentAssertions;
using Ncqrs.Commanding.CommandExecution;
using NUnit.Framework;
using Ncqrs.Commanding;
using Ncqrs.Domain;
using Rhino.Mocks;

namespace Ncqrs.Tests.Commanding.CommandExecution
{
    [TestFixture]
    public class CreatingCommandExecutorTests
    {
        public class CreateMessageCommand : CommandBase
        {
            public string Text
            {
                get; set;
            }
        }

        public class Message : AggregateRoot
        {
            private string _text;

            public Message(string text)
            {
                _text = text;
            }
        }

        [Test]
        public void Constructing_it_with_creating_action_not_throw()
        {
            Func<CreateMessageCommand, Message> creation = (c) => new Message(c.Text);
            new CreatingCommandExecutor<CreateMessageCommand, Message>(creation);
        }

        [Test]
        public void Constructing_it_with_null_arg_should_throw()
        {
            Action act = () => new CreatingCommandExecutor<CreateMessageCommand, Message>(null);
            act.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void Executing_it_should_create_unit_of_work_with_factory()
        {
            var factoryMock = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            var contextMock = MockRepository.GenerateMock<IUnitOfWorkContext>();

            factoryMock.Expect(t => t.CreateUnitOfWork()).Repeat.Once().Return(contextMock);

            Func<CreateMessageCommand, Message> creation = (c) => new Message(c.Text);
            var executor = new CreatingCommandExecutor<CreateMessageCommand, Message>(creation, factoryMock);

            var cmd = new CreateMessageCommand();
            executor.Execute(cmd);

            factoryMock.VerifyAllExpectations();
        }

        [Test]
        public void Executing_it_should_commit_unit_of_work()
        {
            var factoryMock = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            var contextMock = MockRepository.GenerateMock<IUnitOfWorkContext>();

            factoryMock.Stub(t => t.CreateUnitOfWork()).Return(contextMock);
            contextMock.Expect(t => t.Accept()).Repeat.Once();

            Func<CreateMessageCommand, Message> creation = (c) => new Message(c.Text);
            var executor = new CreatingCommandExecutor<CreateMessageCommand, Message>(creation, factoryMock);

            var cmd = new CreateMessageCommand();
            executor.Execute(cmd);

            contextMock.VerifyAllExpectations();
        }

        [Test]
        public void Executing_it_should_invoke_action()
        {
            var invoked = false;

            var factoryMock = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            var contextMock = MockRepository.GenerateMock<IUnitOfWorkContext>();

            factoryMock.Stub(t => t.CreateUnitOfWork()).Return(contextMock);
            contextMock.Expect(t => t.Accept()).Repeat.Once();

            Func<CreateMessageCommand, Message> creation = (c) =>
            {
                invoked = true;
                return new Message(c.Text);
            };

            var executor = new CreatingCommandExecutor<CreateMessageCommand, Message>(creation, factoryMock);

            var cmd = new CreateMessageCommand();
            executor.Execute(cmd);

            invoked.Should().BeTrue();
        }
    }
}
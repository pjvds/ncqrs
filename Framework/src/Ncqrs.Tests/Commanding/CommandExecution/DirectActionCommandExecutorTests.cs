using System;
using FluentAssertions;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Domain;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Tests.Commanding.CommandExecution
{
    [TestFixture]
    public class DirectActionCommandExecutorTests
    {
        public class ChangeMessageTextCommand : CommandBase
        {
            public string Text
            {
                get;
                set;
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
        public void Constructing_it_with_null_arg_for_getId_function_should_throw()
        {
            Action<Message, ChangeMessageTextCommand> anAction = (m, c) => c.GetType();

            Action act = () => new DirectActionCommandExecutor<ChangeMessageTextCommand, Message>(null, anAction);
            act.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void Constructing_it_with_null_arg_for_function_should_throw()
        {
            Func<ChangeMessageTextCommand, Guid> getId = (c) => new Guid();

            Action act = () => new DirectActionCommandExecutor<ChangeMessageTextCommand, Message>(getId, null);
            act.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void Constructing_it_with_correct_args_should_not_throw()
        {
            Func<ChangeMessageTextCommand, Guid> getId = (c) => new Guid();
            Action<Message, ChangeMessageTextCommand> anAction = (m, c) => c.GetType();
            Assert.DoesNotThrow(()=>new DirectActionCommandExecutor<ChangeMessageTextCommand, Message>(getId, anAction));
        }

        [Test]
        public void Executing_it_should_create_unit_of_work_with_factory()
        {
            var factoryMock = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            var contextMock = MockRepository.GenerateMock<IUnitOfWorkContext>();

            factoryMock.Expect(t => t.CreateUnitOfWork(Guid.NewGuid())).IgnoreArguments().Repeat.Once().Return(contextMock);

            Func<ChangeMessageTextCommand, Guid> getId = (c) => new Guid();
            Action<Message, ChangeMessageTextCommand> anAction = (m, c) => c.GetType();
            var executor = new DirectActionCommandExecutor<ChangeMessageTextCommand, Message>(getId, anAction, factoryMock);

            var cmd = new ChangeMessageTextCommand();
            executor.Execute(cmd);

            factoryMock.VerifyAllExpectations();
        }

        [Test]
        public void Executing_it_should_commit_unit_of_work()
        {
            var factoryMock = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            var contextMock = MockRepository.GenerateMock<IUnitOfWorkContext>();

            factoryMock.Stub(t => t.CreateUnitOfWork(Guid.NewGuid())).IgnoreArguments().Return(contextMock);
            contextMock.Expect(t => t.Accept()).Repeat.Once();

            Func<ChangeMessageTextCommand, Guid> getId = (c) => new Guid();
            Action<Message, ChangeMessageTextCommand> anAction = (m, c) => c.GetType();
            var executor = new DirectActionCommandExecutor<ChangeMessageTextCommand, Message>(getId, anAction, factoryMock);

            var cmd = new ChangeMessageTextCommand();
            executor.Execute(cmd);

            contextMock.VerifyAllExpectations();
        }

        [Test]
        public void Executing_it_should_invoke_action()
        {
            var invoked = false;

            var factoryMock = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            var contextMock = MockRepository.GenerateMock<IUnitOfWorkContext>();

            factoryMock.Stub(t => t.CreateUnitOfWork(Guid.NewGuid())).IgnoreArguments().Return(contextMock);
            contextMock.Expect(t => t.Accept()).Repeat.Once();

            Action<Message, ChangeMessageTextCommand> anAction = (m, c) =>
            {
                invoked = true;
            };

            Func<ChangeMessageTextCommand, Guid> getId = (c) => new Guid();
            var executor = new DirectActionCommandExecutor<ChangeMessageTextCommand, Message>(getId, anAction, factoryMock);

            var cmd = new ChangeMessageTextCommand();
            executor.Execute(cmd);

            invoked.Should().BeTrue();
        }

        [Test]
        public void Executing_it_should_invoke_getId()
        {
            var invoked = false;

            var factoryMock = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            var contextMock = MockRepository.GenerateMock<IUnitOfWorkContext>();

            factoryMock.Stub(t => t.CreateUnitOfWork(Guid.NewGuid())).IgnoreArguments().Return(contextMock);
            contextMock.Expect(t => t.Accept()).Repeat.Once();

            Action<Message, ChangeMessageTextCommand> anAction = (m, c) => c.GetType();
            Func<ChangeMessageTextCommand, Guid> getId = (c) =>
            {
                invoked = true;
                return Guid.NewGuid();
            };
            var executor = new DirectActionCommandExecutor<ChangeMessageTextCommand, Message>(getId, anAction, factoryMock);

            var cmd = new ChangeMessageTextCommand();
            executor.Execute(cmd);

            invoked.Should().BeTrue();
        }

        [Test]
        public void Executing_it_should_GetById_of_context_to_get_agg_root()
        {
            var id = Guid.NewGuid();

            var factoryMock = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            var contextMock = MockRepository.GenerateMock<IUnitOfWorkContext>();

            factoryMock.Stub(t => t.CreateUnitOfWork(Guid.NewGuid())).IgnoreArguments().Return(contextMock);
            contextMock.Expect(t => t.GetById(typeof(Message),id, null)).Return(new Message("Hello world"));

            Func<ChangeMessageTextCommand, Guid> getId = (c) => id;
            Action<Message, ChangeMessageTextCommand> anAction = (m, c) => c.GetType();
            var executor = new DirectActionCommandExecutor<ChangeMessageTextCommand, Message>(getId, anAction, factoryMock);

            var cmd = new ChangeMessageTextCommand();
            executor.Execute(cmd);

            contextMock.VerifyAllExpectations();
        }
    }
}

using System;
using FluentAssertions;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Domain;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.NServiceBus.Tests
{
    [TestFixture]
    public class NsbCommandServiceTests
    {
        [Test]
        public void Executing_not_mapped_command_with_no_registered_executors_should_cause_exception()
        {
            var sut = new NsbCommandService();
            Action act = () => sut.Execute(new NotMappedCommand());
            act.ShouldThrow<ExecutorForCommandNotFoundException>();
        }

        [Test]
        public void Mapped_command_with_no_registered_executors_should_be_executed_using_mapped_executor()
        {
            var sut = new NsbCommandService();
            sut.Execute(new MappedCommand());
        }

        [Test]
        public void Mapped_command_with_registered_executors_should_be_executed_using_registered_executor()
        {
            var executor = MockRepository.GenerateMock<ICommandExecutor<MappedCommand>>();
            var sut = new NsbCommandService();
            sut.RegisterExecutor(executor);
            var command = new MappedCommand();
            sut.Execute(command);
            
            executor.AssertWasCalled(x => x.Execute(command));
        }

        [Test]
        public void Not_mapped_command_with_registered_executors_should_be_executed_using_registered_executor()
        {
            var executor = MockRepository.GenerateMock<ICommandExecutor<NotMappedCommand>>();
            var sut = new NsbCommandService();
            sut.RegisterExecutor(executor);
            var command = new NotMappedCommand();
            sut.Execute(command);

            executor.AssertWasCalled(x => x.Execute(command));
        }

        public class NotMappedCommand : CommandBase
        {            
        }

        [MapsToAggregateRootConstructor("Ncqrs.NServiceBus.Tests.NsbCommandServiceTests+TestAgggregateRoot, Ncqrs.NServiceBus.Tests")]
        public class MappedCommand : CommandBase
        {
        }

        public class TestAgggregateRoot : AggregateRoot
        {            
        }
    }
}

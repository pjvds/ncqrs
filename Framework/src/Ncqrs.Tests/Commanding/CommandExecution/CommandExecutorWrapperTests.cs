using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using NUnit.Framework;

namespace Ncqrs.Tests.Commanding.CommandExecution
{
    [TestFixture]
    public class CommandExecutorWrapperTests
    {
        public class TheCommand : ICommand
        {
            public Guid CommandIdentifier
            {
                get; set;
            }

            public long? KnownVersion
            {
                get; set;
            }
        }

        [Test]
        public void Constructing_it_with_a_null_action_should_throw()
        {
            Action<ICommand> nullAction = null; 
            
            Action act = ()=> new CommandExecutorWrapper<ICommand>(nullAction);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("action");
        }

        [Test]
        public void Executig_it_should_redirect_call_to_initialized_action()
        {
            bool redirected = false;
            Action<ICommand> action = (c) => redirected = true;
            var cmd = new TheCommand();

            var target = new CommandExecutorWrapper<ICommand>(action);
            target.Execute(cmd);

            redirected.Should().BeTrue();
        }

        [Test]
        public void Executig_it_should_redirect_the_given_command_to_initialized_action()
        {
            var theCommand = new TheCommand();
            Action<ICommand> action = (c) => c.Should().BeSameAs(theCommand);

            var target = new CommandExecutorWrapper<ICommand>(action);
            target.Execute(theCommand);
        }
    }
}

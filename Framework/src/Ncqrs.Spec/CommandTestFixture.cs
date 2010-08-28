using System;
using System.Collections.Generic;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Eventing.Sourcing;
using NUnit.Framework;

namespace Ncqrs.Spec
{
    [Specification]
    [TestFixture] // TODO: Testdriven.net debug runner doesn't recognize inhiret attributes. Use native for now.
    public abstract class CommandTestFixture<TCommand>
        where TCommand : ICommand
    {
        protected Exception CaughtException{ get; private set; }

        protected IEnumerable<ISourcedEvent> PublishedEvents{ get; private set;}

        protected TCommand ExecutedCommand { get; private set; }
        
        protected abstract TCommand WhenExecutingCommand();
        
        protected virtual void SetupDependencies() { }
        protected virtual void Finally() { }

        [Given]
        [SetUp] // TODO: Testdriven.net debug runner doesn't recognize inhiret attributes. Use native for now.
        public void Setup()
        {
            var commandExecutor = BuildCommandExecutor();
            PublishedEvents = new SourcedEvent[0];

            SetupDependencies();
            try
            {
                var command = WhenExecutingCommand();

                using (var context = new EventContext())
                {
                    commandExecutor.Execute(command);

                    ExecutedCommand = command;

                    PublishedEvents = context.Events;
                }
            }
            catch (Exception exception)
            {
                CaughtException = exception;
            }
            finally
            {
                Finally();
            }
        }

        protected abstract ICommandExecutor<TCommand> BuildCommandExecutor();
    }
}

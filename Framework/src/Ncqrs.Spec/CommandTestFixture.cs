using System;
using System.Collections.Generic;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Spec
{
    [Specification]
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

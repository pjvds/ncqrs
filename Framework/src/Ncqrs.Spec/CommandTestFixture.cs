using System;
using System.Collections.Generic;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Eventing;
using NUnit.Framework;

namespace Ncqrs.Spec
{

    public abstract class CommandTestFixture<TCommand>
        : DomainTestFixture<TCommand>
        where TCommand : ICommand
    {

        protected ICommandExecutor<ICommand> CommandExecutor { get; private set; }

        protected override void SetupDependencies()
        {
            base.SetupDependencies();
            CommandExecutor = BuildCommandExecutor();
        }

        protected override void Execute(TCommand command)
        {
            CommandExecutor.Execute(command);
        }

        protected abstract ICommandExecutor<ICommand> BuildCommandExecutor();
    }
}

using System;
using System.Diagnostics.Contracts;

namespace Ncqrs.Commanding.CommandExecution
{
    /// <summary>
    /// Executes a command. This means that the handles 
    /// executes the correct action based on the command.
    /// </summary>
    [ContractClass(typeof(ICommandExecutorContracts<>))]
    public interface ICommandExecutor<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The command to execute. This should not be null.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>command</i> is null.</exception>
        void Execute(TCommand command);
    }

    [ContractClassFor(typeof(ICommandExecutor<>))]
    internal abstract class ICommandExecutorContracts<TCommand> : ICommandExecutor<TCommand> where TCommand : ICommand
    {
        public void Execute(TCommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null, "The command cannot be null.");
        }
    }
}

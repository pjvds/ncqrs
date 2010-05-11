using System;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution
{
    /// <summary>
    /// Represents a command executor.
    /// </summary>
    /// <typeparam name="TCommand">The type of the commands to execute.</typeparam>
    public abstract class CommandExecutorBase<TCommand> : ICommandExecutor<TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The command to execute. This should not be null.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>command</i> is null.</exception>
        public void Execute(TCommand command)
        {
            using (var work = NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork())
            {
                ExecuteWithingUnitOfWorkContext(work);
            }
        }

        /// <summary>
        /// Executes the command withing an unit of work context.
        /// <remarks>Make sure you call <see cref="IUnitOfWork.Accept"/> to accept the changes that has been made in the context.</remarks>
        /// </summary>
        /// <param name="work">The work.</param>
        protected abstract void ExecuteWithingUnitOfWorkContext(IUnitOfWorkContext work);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commands;
using System.Diagnostics.Contracts;

namespace Ncqrs.CommandHandling
{
    /// <summary>
    /// Executes a command. This means that the handles 
    /// executes the correct action based on the command.
    /// </summary>
    [ContractClass(typeof(ICommandExecutorContracts))]
    public interface ICommandExecutor
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The command to execute. This should not be null.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>command</i> is null.</exception>
        void Execute(ICommand command);
    }

    [ContractClassFor(typeof(ICommandExecutor))]
    internal class ICommandExecutorContracts : ICommandExecutor
    {
        public void Execute(ICommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null, "The command cannot be null.");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commands;
using System.Diagnostics.Contracts;

namespace Ncqrs.CommandHandling
{
    /// <summary>
    /// Handles a command. This means that the handles 
    /// executes the correct action based on the command.
    /// </summary>
    [ContractClass(typeof(ICommandHandlerContracts))]
    public interface ICommandHandler
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="message">The command to execute. This should not be null.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>command</i> is null.</exception>
        void Execute(ICommand command);
    }

    [ContractClassFor(typeof(ICommandHandler))]
    internal class ICommandHandlerContracts : ICommandHandler
    {
        public void Execute(ICommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null, "The command cannot be null.");
        }
    }
}

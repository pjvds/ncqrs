using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commands;

namespace Ncqrs.CommandHandling
{
    /// <summary>
    /// Represents a command handler.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Handles a command.
        /// </summary>
        /// <param name="message">The command to handle. This should not be null.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>command</i> is null.</exception>
        void Execute(ICommand command);
    }
}
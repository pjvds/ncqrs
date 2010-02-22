using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commands;

namespace Ncqrs.CommandHandling
{
    public interface ICommandHandlerRegister
    {
        /// <summary>
        /// Determ whether a Handlers exists for a specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns><c>true</c> when a handler exists; otherwise <c>false.</c>.</returns>
        Boolean HandlerExists(ICommand command);

        /// <summary>
        /// Gets the handler for the specified <see cref="ICommand"/>.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>A instance of an <see cref="ICommandHandler"/> if exists; otherwise, <c>null</c>.</returns>
        ICommandHandler GetHandler(ICommand command);
    }
}

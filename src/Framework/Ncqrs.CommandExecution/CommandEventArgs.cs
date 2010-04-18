using System;
using Ncqrs.Commands;

namespace Ncqrs.CommandExecution
{
    /// <summary>
    /// Contains the command event data.
    /// </summary>
    public class CommandEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <value>The command.</value>
        public ICommand Command
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandEventArgs"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandEventArgs(ICommand command)
        {
            Command = command;
        }
    }
}

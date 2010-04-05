using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commands;

namespace Ncqrs.CommandHandling
{
    /// <summary>
    /// Contains the command event data.
    /// </summary>
    public class CommandEventArgs
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

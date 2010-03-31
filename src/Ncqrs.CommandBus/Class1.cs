using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commands;

namespace Ncqrs.CommandBus
{
    /// <summary>
    /// A bus that can publish command to handlers.
    /// </summary>
    public interface ICommandBus
    {
        /// <summary>
        /// Publishes the command to the handlers.
        /// </summary>
        /// <param name="message">The message to publish.</param>
        /// <exception cref="ArgumentNullException">Thrown when <i>message</i> was null.</exception>
        /// <exception cref="NoHandlerRegisteredForMessageException">Thrown when no handler was found for the specified message.</exception>
        void Publish(ICommand command);

        /// <summary>
        /// Publishes the messages to the handlers.
        /// </summary>
        /// <param name="messages">The messages to publish.</param>
        /// <exception cref="ArgumentNullException">Thrown when <i>messages</i> was null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when a instance in <i>messages</i> was null.</exception>
        /// <exception cref="NoHandlerRegisteredForMessageException">Thrown when no handler was found for one of the specified messages.</exception>
        void Publish(IEnumerable<ICommand> commands);
    }

}

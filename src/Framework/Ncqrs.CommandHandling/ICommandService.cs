using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commands;
using System.Diagnostics.Contracts;

namespace Ncqrs.CommandHandling
{
    /// <summary>
    /// A service that dispatches Command objects to their appropriate CommandHandler. CommandHandlers can subscribe and
    /// unsubscribe to specific types of commands. Only a single handler may be subscribed for a single type of command at any time.
    /// </summary>
    [ContractClass(typeof(ICommandServiceContracts))]
    public interface ICommandService
    {
        /// <summary>
        /// Execute a <see cref="ICommand"/> by giving it to the registered <see cref="ICommandHandler"/>.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>command</i> was a <c>null</c> dereference.</exception>
        /// <exception cref="CommandHandlerNotFoundException">Occurs when the <see cref="ICommandHandler"/> was not found for on the given <see cref="ICommand"/>.</exception>
        void Execute(ICommand command);

        /// <summary>
        /// Executes multiple <see cref="ICommand"/>'s by giving them to the registered <see cref="ICommandHandler"/>.
        /// </summary>
        /// <param name="commands">The commands to execute.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>command</i> was a <c>null</c> dereference.</exception>
        /// <exception cref="CommandHandlerNotFoundException">Occurs when a <see cref="ICommandHandler"/> was not found for on of the given <see cref="ICommand"/>'s.</exception>
        void Execute(IEnumerable<ICommand> commands);

        /// <summary>
        /// Registers the handler for the specified command type. The handler will be called for every command of the specified type.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="handler">The handler that will be called for every command of the specified type.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>commandType</i> or <i>handler</i> was a <c>null</c> dereference.</exception>
        void RegisterHandler(Type commandType, ICommandHandler handler);

        /// <summary>
        /// Registers the handler for the specified command type. The handler will be called for every command of the specified type.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        /// <param name="handler">The handler that will be called for every command of the specified type.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>commandType</i> or <i>handler</i> was a <c>null</c> dereference.</exception>
        void RegisterHandler<TCommand>(ICommandHandler handler) where TCommand : ICommand;

        /// <summary>
        /// Unregisters the handler of the specified command type. The handler will not be called any more.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="handler">The handler to unregister.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>commandType</i> or <i>handler</i> was a <c>null</c> dereference.</exception>
        void UnregisterHandler(Type commandType, ICommandHandler handler);
    }

    [ContractClassFor(typeof(ICommandService))]
    internal sealed class ICommandServiceContracts : ICommandService
    {
        public void Execute(ICommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null, "The command cannot be null.");
        }

        public void Execute(IEnumerable<ICommand> commands)
        {
            Contract.Requires<ArgumentNullException>(commands != null, "The command cannot be null.");
            Contract.Requires<ArgumentException>(commands.Count() > 0, "There should be at least on command to execute.");
        }

        public void RegisterHandler(Type commandType, ICommandHandler handler)
        {
            Contract.Requires<ArgumentNullException>(commandType != null, "The commandType cannot be null.");
            Contract.Requires<ArgumentNullException>(handler != null);
        }

        public void UnregisterHandler(Type commandType, ICommandHandler handler)
        {
            Contract.Requires<ArgumentNullException>(commandType != null, "The commandType cannot be null.");
            Contract.Requires<ArgumentNullException>(typeof(ICommand).IsAssignableFrom(commandType), "The commandType should be of a type that implements the ICommand interface.");
            Contract.Requires<ArgumentNullException>(handler != null, "The handler cannot be null.");
        }

        public void RegisterHandler<TCommand>(ICommandHandler handler) where TCommand : ICommand
        {
            Contract.Requires<ArgumentNullException>(handler != null, "The handler cannot be null.");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commands;
using System.Transactions;
using System.Diagnostics.Contracts;
using System.Reflection;
using log4net;

namespace Ncqrs.CommandHandling
{
    /// <summary>
    /// A service that dispatches Command objects to their appropriate CommandHandler. CommandHandlers can subscribe and
    /// unsubscribe to specific types of commands. Only a single handler may be subscribed for a single type of command at any time.
    /// </summary>
    /// <remarks>
    /// This command service is in process and transactional. The transactional system is based on <see cref="System.Transactions"/>. 
    /// Since this service is in process, no additional software of configiration is needed.
    /// </remarks>
    public class TransactionalInProcessCommandService : ICommandService
    {
        private readonly Dictionary<Type, ICommandHandler> _handlers = new Dictionary<Type, ICommandHandler>();
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Execute a <see cref="ICommand"/> by giving it to the registered <see cref="ICommandHandler"/>.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>command</i> was a <c>null</c> dereference.</exception>
        /// <exception cref="CommandHandlerNotFoundException">Occurs when the <see cref="ICommandHandler"/> was not found for on the given <see cref="ICommand"/>.</exception>
        public void Execute(ICommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            Type commandType = command.GetType();

            Log.InfoFormat("Received {0} command and will now start processing it.", commandType.FullName);

            using (var transaction = new TransactionScope())
            {
                ICommandHandler handler = null;

                if (!_handlers.TryGetValue(commandType, out handler))
                {
                    throw new CommandHandlerNotFoundException(commandType);
                }

                Log.DebugFormat("Found commandhandler {0} to handle the {1} command. Will start executing it now.", handler.GetType().FullName, commandType.FullName);

                handler.Execute(command);

                Log.DebugFormat("Handler execution complete.");

                transaction.Complete();
            }

            Log.InfoFormat("Finished processing {0}.", commandType.FullName);
        }

        /// <summary>
        /// Executes multiple <see cref="ICommand"/>'s by giving them to the registered <see cref="ICommandHandler"/>.
        /// </summary>
        /// <param name="commands">The commands to execute.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>command</i> was a <c>null</c> dereference.</exception>
        /// <exception cref="CommandHandlerNotFoundException">Occurs when a <see cref="ICommandHandler"/> was not found for on of the given <see cref="ICommand"/>'s.</exception>
        public void Execute(IEnumerable<ICommand> commands)
        {
            Contract.Requires(commands != null);

            using (var transaction = new TransactionScope())
            {
                foreach (var command in commands)
                {
                    Execute(command);
                }

                transaction.Complete();
            }
        }

        /// <summary>
        /// Registers the handler for the specified command type. The handler will be called for every command of the specified type.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        /// <param name="handler">The handler that will be called for every command of the specified type.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>commandType</i> or <i>handler</i> was a <c>null</c> dereference.</exception>
        public void RegisterHandler<TCommand>(ICommandHandler handler) where TCommand : ICommand
        {
            RegisterHandler(typeof(TCommand), handler);
        }

        /// <summary>
        /// Registers the handler for the specified command type. The handler will be called for every command of the specified type.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="handler">The handler that will be called for every command of the specified type.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>commandType</i> or <i>handler</i> was a <c>null</c> dereference.</exception>
        public void RegisterHandler(Type commandType, ICommandHandler handler)
        {
            Contract.Requires<ArgumentNullException>(commandType != null);
            Contract.Requires<ArgumentNullException>(handler != null);

            _handlers.Add(commandType, handler);
        }


        /// <summary>
        /// Unregisters the handler of the specified command type. The handler will not be called any more.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="handler">The handler to unregister.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>commandType</i> or <i>handler</i> was a <c>null</c> dereference.</exception>
        public void UnregisterHandler(Type commandType, ICommandHandler handler)
        {
            throw new NotImplementedException();
        }
    }
}
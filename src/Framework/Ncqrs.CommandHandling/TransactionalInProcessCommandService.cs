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
    /// A service that dispatches Command objects to their appropriate Commandexecutor. Commandexecutors can subscribe and
    /// unsubscribe to specific types of commands. Only a single executor may be subscribed for a single type of command at any time.
    /// </summary>
    /// <remarks>
    /// This command service is in process and transactional. The transactional system is based on <see cref="System.Transactions"/>. 
    /// Since this service is in process, no additional software of configiration is needed.
    /// </remarks>
    public class TransactionalInProcessCommandService : ICommandExecutor
    {
        private readonly Dictionary<Type, ICommandExecutor> _executors = new Dictionary<Type, ICommandExecutor>();
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Execute a <see cref="ICommand"/> by giving it to the registered <see cref="ICommandExecutor"/>.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>command</i> was a <c>null</c> dereference.</exception>
        /// <exception cref="CommandExecutorNotFoundException">Occurs when the <see cref="ICommandExecutor"/> was not found for on the given <see cref="ICommand"/>.</exception>
        public void Execute(ICommand command)
        {
            Type commandType = command.GetType();

            Log.InfoFormat("Received {0} command and will now start processing it.", commandType.FullName);

            using (var transaction = new TransactionScope())
            {
                ICommandExecutor executor = null;

                if (!_executors.TryGetValue(commandType, out executor))
                {
                    throw new CommandExecutorNotFoundException(commandType);
                }

                Log.DebugFormat("Found command executor {0} to execute the {1} command. Will start executing it now.", executor.GetType().FullName, commandType.FullName);

                executor.Execute(command);

                Log.DebugFormat("Execution complete.");

                transaction.Complete();
            }

            Log.InfoFormat("Finished processing {0}.", commandType.FullName);
        }

        /// <summary>
        /// Executes multiple <see cref="ICommand"/>'s by giving them to the registered <see cref="ICommandExecutor"/>.
        /// </summary>
        /// <param name="commands">The commands to execute.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>command</i> was a <c>null</c> dereference.</exception>
        /// <exception cref="CommandExecutorNotFoundException">Occurs when a <see cref="ICommandExecutor"/> was not found for on of the given <see cref="ICommand"/>'s.</exception>
        public void Execute(IEnumerable<ICommand> commands)
        {
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
        /// Registers the executor for the specified command type. The executor will be called for every command of the specified type.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        /// <param name="executor">The executor that will be called for every command of the specified type.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>commandType</i> or <i>executor</i> was a <c>null</c> dereference.</exception>
        public void RegisterExecutor<TCommand>(ICommandExecutor executor) where TCommand : ICommand
        {
            RegisterExecutor(typeof(TCommand), executor);
        }

        /// <summary>
        /// Registers the executor for the specified command type. The executor will be called for every command of the specified type.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="executor">The executor that will be called for every command of the specified type.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>commandType</i> or <i>executor</i> was a <c>null</c> dereference.</exception>
        public void RegisterExecutor(Type commandType, ICommandExecutor executor)
        {
            _executors.Add(commandType, executor);
        }


        /// <summary>
        /// Unregisters the executor of the specified command type. The executor will not be called any more.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="executor">The executor to unregister.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>commandType</i> or <i>executor</i> was a <c>null</c> dereference.</exception>
        /// <exception cref="InvalidOperationException">Occurs when the <i>executor</i> is not the same as the registered executor for the specified command type.</exception>
        public void UnregisterExecutor(Type commandType, ICommandExecutor executor)
        {
            ICommandExecutor registeredExecutor = null;

            if (_executors.TryGetValue(commandType, out registeredExecutor))
            {
                if (executor != registeredExecutor)
                    throw new InvalidOperationException("The specified executor does not match with the registered executor for command type " + commandType.FullName + ".");

                _executors.Remove(commandType);
            }
        }
    }
}
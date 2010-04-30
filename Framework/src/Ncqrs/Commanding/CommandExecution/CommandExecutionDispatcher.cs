using System;
using System.Reflection;
using log4net;

namespace Ncqrs.Commanding.CommandExecution
{
    /// <summary>
    /// A command executor that dispatch the command execution to other command executors.
    /// </summary>
    public abstract class CommandExecutionDispatcher : ICommandExecutor
    {
        /// <summary>
        /// The logger to use to log messages.
        /// </summary>
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

            ICommandExecutor executor = GetCommandExecutorForCommand(commandType);

            if (executor == null)
            {
                throw new CommandExecutorNotFoundException(commandType);
            }

            Log.DebugFormat("Found command executor {0} to execute the {1} command. Will start executing it now.", executor.GetType().FullName, commandType.FullName);

            executor.Execute(command);

            Log.DebugFormat("Execution complete.");
        }

        /// <summary>
        /// Gets the command executor for command.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>A command executor to use to execute the command or <c>null</c> if not found.</returns>
        protected abstract ICommandExecutor GetCommandExecutorForCommand(Type commandType);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commands;
using System.Transactions;
using System.Diagnostics.Contracts;
using System.Reflection;
using log4net;

namespace Ncqrs.CommandExecution
{
    /// <summary>
    /// A service that dispatches Command objects to their appropriate Commandexecutor. Commandexecutors can subscribe and
    /// unsubscribe to specific types of commands. Only a single executor may be subscribed for a single type of command at any time.
    /// </summary>
    /// <remarks>
    /// This command service is in process and transactional. The transactional system is based on <see cref="System.Transactions"/>. 
    /// Since this service is in process, no additional software of configiration is needed.
    /// </remarks>
    public class InProcessCommandExecutionDispatcher : CommandExecutionDispatcher
    {
        private readonly Dictionary<Type, ICommandExecutor> _executors = new Dictionary<Type, ICommandExecutor>();
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

        /// <summary>
        /// Gets the command executor for command.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>
        /// A command executor to use to execute the command or <c>null</c> if not found.
        /// </returns>
        protected override ICommandExecutor GetCommandExecutorForCommand(Type commandType)
        {
            ICommandExecutor result = null;
            _executors.TryGetValue(commandType, out result);

            return result;
        }
    }
}
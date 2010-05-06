using System;
using System.Collections.Generic;
using System.Reflection;
using Ncqrs.Commanding.CommandExecution;

namespace Ncqrs.Commanding.ServiceModel
{
    public abstract class CommandServiceBase : ICommandService
    {
        protected readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Dictionary<Type, ICommandExecutor> _executors = new Dictionary<Type, ICommandExecutor>();
        private readonly List<ICommandServiceInterceptor> _interceptors = new List<ICommandServiceInterceptor>(0);

        /// <summary>
        /// Execute a <see cref="ICommand"/> by giving it to the registered <see cref="ICommandExecutor"/>.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>command</i> was a <c>null</c> dereference.</exception>
        /// <exception cref="CommandExecutorNotFoundException">Occurs when the <see cref="ICommandExecutor"/> was not found for on the given <see cref="ICommand"/>.</exception>
        public virtual void Execute(ICommand command)
        {
            Type commandType = command.GetType();
            var context = new CommandServiceExecutionContext(command);

            try
            {
                // Get executor for the command.
                ICommandExecutor executor = GetCommandExecutorForCommand(commandType);
                context.TheCommandExecutor = executor;

                // Call OnBeforeExecution on every interceptor.
                _interceptors.ForEach(i => i.OnBeforeExecution(context));

                // When we couldn't find an executore, throw exception.
                if (executor == null)
                {
                    throw new CommandExecutorNotFoundException(commandType);
                }

                // Execute the command.
                executor.Execute(command);

                // Set mark that the command is executed.
                context.IsExecuted = true;
            }
            catch(Exception caught)
            {
                // There was an exception, add it to the context
                // and retrow.
                context.Exception = caught;
                throw;
            }
            finally
            {
                // Call OnAfterExecution on every interceptor.
                _interceptors.ForEach(i=>i.OnAfterExecution(context));
            }
        }

        /// <summary>
        /// Registers the executor for the specified command type. The executor will be called for every command of the specified type.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        /// <param name="executor">The executor that will be called for every command of the specified type.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>commandType</i> or <i>executor</i> was a <c>null</c> dereference.</exception>
        protected virtual void RegisterExecutor<TCommand>(ICommandExecutor executor) where TCommand : ICommand
        {
            RegisterExecutor(typeof(TCommand), executor);
        }

        /// <summary>
        /// Registers the executor for the specified command type. The executor will be called for every command of the specified type.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="executor">The executor that will be called for every command of the specified type.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>commandType</i> or <i>executor</i> was a <c>null</c> dereference.</exception>
        protected virtual void RegisterExecutor(Type commandType, ICommandExecutor executor)
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
        protected virtual void UnregisterExecutor(Type commandType, ICommandExecutor executor)
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
        protected virtual ICommandExecutor GetCommandExecutorForCommand(Type commandType)
        {
            ICommandExecutor result;
            _executors.TryGetValue(commandType, out result);

            return result;
        }

        protected virtual void AddInterceptor(ICommandServiceInterceptor interceptor)
        {
            if (!_interceptors.Contains(interceptor))
            {
                _interceptors.Add(interceptor);
            }
        }

        protected virtual void RemoveInterceptor(ICommandServiceInterceptor interceptor)
        {
            _interceptors.Remove(interceptor);
        }
    }
}

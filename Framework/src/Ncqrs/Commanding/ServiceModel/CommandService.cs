using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using Ncqrs.Commanding.CommandExecution;

namespace Ncqrs.Commanding.ServiceModel
{
    public class CommandService : ICommandService
    {
        protected readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Dictionary<Type, Action<ICommand>> _executors = new Dictionary<Type, Action<ICommand>>();
        private readonly List<ICommandServiceInterceptor> _interceptors = new List<ICommandServiceInterceptor>(0);

        /// <summary>
        /// Execute a <see cref="ICommand"/> by giving it to the registered <see cref="ICommandExecutor{TCommand}"/>.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>command</i> was a <c>null</c> dereference.</exception>
        /// <exception cref="ExecutorForCommandNotFoundException">Occurs when the <see cref="ICommandExecutor{TCommand}"/> was not found for on the given <see cref="ICommand"/>.</exception>
        public virtual void Execute(ICommand command)
        {
            Type commandType = command.GetType();
            var context = new CommandContext(command);

            try
            {
                // Call OnBeforeExecution on every interceptor.
                _interceptors.ForEach(i => i.OnBeforeBeforeExecutorResolving(context));

                // Get executor for the command.
                var executor = GetCommandExecutorForCommand(commandType);
                context.ExecutorResolved = executor != null;

                // Call OnBeforeExecution on every interceptor.
                _interceptors.ForEach(i => i.OnBeforeExecution(context));

                // When we couldn't find an executore, throw exception.
                if (executor == null)
                {
                    throw new ExecutorForCommandNotFoundException(commandType);
                }

                // Set mark that the command executor has been called for this command.
                context.ExecutorHasBeenCalled = true;

                // Execute the command.
                executor(command);
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

        public virtual void RegisterExecutor(Type commandType, ICommandExecutor<ICommand> executor)
        {
            RegisterExecutor<ICommand>(commandType, executor);
        }

        public virtual void RegisterExecutor<TCommand>(Type commandType, ICommandExecutor<TCommand> executor) where TCommand : ICommand
        {
            Contract.Requires<ArgumentOutOfRangeException>(typeof(TCommand).IsAssignableFrom(commandType));
            if (_executors.ContainsKey(commandType)) return;
            Action<ICommand> action = (cmd) => executor.Execute((TCommand) cmd);
            _executors.Add(commandType, action);
        }

        /// <summary>
        /// Registers the executor for the specified command type. The executor will be called for every command of the specified type.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        /// <param name="executor">The executor that will be called for every command of the specified type.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>commandType</i> or <i>executor</i> was a <c>null</c> dereference.</exception>
        public virtual void RegisterExecutor<TCommand>(ICommandExecutor<TCommand> executor) where TCommand : ICommand
        {
            if (_executors.ContainsKey(typeof(TCommand))) return;
            Action<ICommand> action = (cmd) => executor.Execute((TCommand) cmd);
            _executors.Add(typeof(TCommand), action);
        }

        /// <summary>
        /// Unregisters the executor of the specified command type. The executor will not be called any more.
        /// </summary>
        /// <param name="executor">The executor to unregister.</param>
        /// <exception cref="ArgumentNullException">Occurs when the <i>commandType</i> or <i>executor</i> was a <c>null</c> dereference.</exception>
        /// <exception cref="InvalidOperationException">Occurs when the <i>executor</i> is not the same as the registered executor for the specified command type.</exception>
        public virtual void UnregisterExecutor<TCommand>() where TCommand : ICommand
        {
            _executors.Remove(typeof (TCommand));
        }

        /// <summary>
        /// Gets the command executor for command.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>
        /// A command executor to use to execute the command or <c>null</c> if not found.
        /// </returns>
        protected virtual Action<ICommand> GetCommandExecutorForCommand(Type commandType)
        {
            Action<ICommand> result;
            _executors.TryGetValue(commandType, out result);

            return result;
        }

        /// <summary>
        /// Adds the interceptor. The interceptor will be called on every
        /// command execution.
        /// </summary>
        /// <remarks>
        /// When the interceptor was already added to this command service, it
        /// is skipped. That means that it is not added twice.
        /// </remarks>
        /// <param name="interceptor">The interceptor to add.</param>
        public virtual void AddInterceptor(ICommandServiceInterceptor interceptor)
        {
            if (!_interceptors.Contains(interceptor))
            {
                _interceptors.Add(interceptor);
            }
        }

        /// <summary>
        /// Removes the interceptor. The interceptor will not be called anymore.
        /// </summary>
        /// <param name="interceptor">The interceptor to remove.</param>
        public virtual void RemoveInterceptor(ICommandServiceInterceptor interceptor)
        {
            _interceptors.Remove(interceptor);
        }
    }
}

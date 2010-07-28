using System;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    /// <summary>
    /// A command handler that execute an action based on the mapping of a command.
    /// </summary>
    public class AttributeMappedCommandExecutor<TCommand> : ICommandExecutor<TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The command to execute. This should not be null.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>command</i> is null.</exception>
        void ICommandExecutor<TCommand>.Execute(TCommand command)
        {
            var factory = new AttributeBasedMappingFactory();
            var executor = factory.CreateExecutorForCommand<TCommand>();

            if (command.GetType().IsDefined(typeof(TransactionalAttribute), true))
            {
                executor = new TransactionalCommandExecutorWrapper<TCommand>(executor);
            }

            executor.Execute(command);
        }
    }
}
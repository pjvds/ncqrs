using System;
using Ncqrs.Commands;
using Ncqrs.Domain.Storage;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Ncqrs.Commands.Attributes;

namespace Ncqrs.CommandExecution.AutoMapping
{
    /// <summary>
    /// A command handler that execute an action based on the mapping of a command.
    /// </summary>
    public class AutoMappingCommandExecutor : ICommandExecutor
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The command to execute. This should not be null.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>command</i> is null.</exception>
        public void Execute(ICommand command)
        {
            var factory = new ActionFactory();
            ICommandExecutor executor = factory.CreateExecutorForCommand(command);

            if (command.GetType().GetCustomAttributes(typeof(TransactionalAttribute), true).Length > 0)
            {
                executor = new TransactionalCommandExecutorWrapper(executor);
            }

            executor.Execute(command);
        }
    }
}

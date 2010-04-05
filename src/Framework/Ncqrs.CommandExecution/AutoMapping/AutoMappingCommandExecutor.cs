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
    /// <typeparam name="T">The type of the command.</typeparam>
    public class AutoMappingCommandExecutor<T> : CommandExecutorBase<T> where T : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMappingCommandExecutor&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="domainRepository">The domain repository.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>domainRepository</i> is <c>null</c>.</exception>
        public AutoMappingCommandExecutor(IDomainRepository domainRepository)
            : base(domainRepository)
        {
            Contract.Requires<ArgumentNullException>(domainRepository != null, "The domainRepository cannot be null.");
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The command to execute. This should not be null.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>command</i> is null.</exception>
        public override void Execute(T command)
        {
            var factory = new ActionFactory(Repository);
            ICommandExecutor executor = factory.CreateExecutorForCommand(command);

            if (command.GetType().GetCustomAttributes(typeof(TransactionalAttribute), true).Length > 0)
            {
                executor = new TransactionalCommandExecutorWrapper(executor);
            }

            executor.Execute(command);
        }
    }
}

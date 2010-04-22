using System;
using Ncqrs.Commands;
using Ncqrs.Domain.Storage;
using System.Collections.Generic;

namespace Ncqrs.CommandHandling.AutoMapping
{
    /// <summary>
    /// A command handler that execute an action based on the mapping of a command.
    /// </summary>
    /// <typeparam name="T">The type of the command.</typeparam>
    public class AutoMappingCommandHandler<T> : CommandHandler<T> where T : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMappingCommandHandler&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="domainRepository">The domain repository.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>domainRepository</i> is <c>null</c>.</exception>
        public AutoMappingCommandHandler(IDomainRepository domainRepository)
            : base(domainRepository)
        {
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The command to execute. This should not be null.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>command</i> is null.</exception>
        public override void Execute(T command)
        {
            var factory = new ActionFactory(Repository);
            var action = factory.CreateActionForCommand(command);
            action.Execute();
        }
    }
}

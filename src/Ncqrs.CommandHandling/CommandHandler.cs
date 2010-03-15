using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using Ncqrs.Commands;
using Ncqrs.Domain.Storage;

namespace Ncqrs.CommandHandling
{
    /// <summary>
    /// Represents a command handler.
    /// </summary>
    /// <typeparam name="T">The type of the command to handle.</typeparam>
    public abstract class CommandHandler<T> : ICommandHandler where T : ICommand
    {
        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <value>The repository.</value>
        protected IDomainRepository Repository
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler&lt;T&gt;"/> class.
        /// </summary>
        protected CommandHandler(IDomainRepository domainRepository)
        {
            Contract.Requires(domainRepository != null);

            Repository = domainRepository;
        }

        /// <summary>
        /// Handles a command.
        /// </summary>
        /// <param name="message">The command to handle. This should not be null.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>command</i> is null.</exception>
        public abstract void Handle(T command);

        void ICommandHandler.Execute(ICommand command)
        {
            this.Handle((T)command);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using Ncqrs.Commands;
using Ncqrs.Domain.Storage;
using Ncqrs.Domain;

namespace Ncqrs.CommandHandling
{
    /// <summary>
    /// Represents a command handler.
    /// </summary>
    /// <typeparam name="T">The type of the command to handle.</typeparam>
    public abstract class CommandHandler<T> : ICommandHandler where T : ICommand
    {
        private readonly IDomainRepository _repository;

        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <value>The repository.</value>
        protected IDomainRepository Repository
        {
            get
            {
                Contract.Ensures(Contract.Result<IDomainRepository>() != null, "The result should never be null.");
                
                return _repository;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler&lt;T&gt;"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">Occurs when <i>domainRepository</i> is <c>null</c>.</exception>
        protected CommandHandler(IDomainRepository domainRepository)
        {
            Contract.Requires<ArgumentNullException>(domainRepository != null, "The domainRepository cannot be null.");
            Contract.Ensures(Repository == domainRepository, "Repository should be initialized with the given parameter domainRepository.");

            _repository = domainRepository;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The command to execute. This should not be null.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>command</i> is null.</exception>
        public abstract void Execute(T command);

        void ICommandHandler.Execute(ICommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null, "The command cannot be null.");
            Contract.Requires<ArgumentException>(command is T, "The command should be of type " + typeof(T).FullName);

            this.Execute((T)command);
        }
    }
}
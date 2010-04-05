using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using Ncqrs.Commands;
using Ncqrs.Domain.Storage;
using Ncqrs.Domain;

namespace Ncqrs.CommandExecution
{
    /// <summary>
    /// Represents a command executor.
    /// </summary>
    /// <typeparam name="TCommand">The type of the commands to execute.</typeparam>
    public abstract class CommandExecutorBase<TCommand> : ICommandExecutor where TCommand : ICommand
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
        /// Initializes a new instance of the <see cref="CommandExecutorBase&lt;TCommand&gt;"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">Occurs when <i>domainRepository</i> is <c>null</c>.</exception>
        protected CommandExecutorBase(IDomainRepository domainRepository)
        {
            Contract.Requires<ArgumentNullException>(domainRepository != null, "The domainRepository cannot be null.");
            Contract.Ensures(Repository == domainRepository, "Repository should be initialized with the given parameter domainRepository.");

            _repository = domainRepository;
        }

        [ContractInvariantMethod]
        private void ContractInvariants()
        {
            Contract.Invariant(_repository != null, "The _repository member should never be null.");
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The command to execute. This should not be null.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>command</i> is null.</exception>
        public abstract void Execute(TCommand command);

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The command to execute. This should not be null.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>command</i> is null.</exception>
        /// <exception cref="InvalidCastException">Occurs when the <i>command</i> could not be casted to <c>TCommand</c>.</exception>
        void ICommandExecutor.Execute(ICommand command)
        {
            this.Execute((TCommand)command);
        }
    }
}
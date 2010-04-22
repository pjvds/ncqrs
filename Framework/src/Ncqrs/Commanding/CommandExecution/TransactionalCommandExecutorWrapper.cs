using System;
using System.Diagnostics.Contracts;
using System.Transactions;

namespace Ncqrs.Commanding.CommandExecution
{
    /// <summary>
    /// Wraps transactional behavior around the execution of command executor.
    /// </summary>
    /// <remarks>
    /// The transaction logic uses TransactionScope.
    /// </remarks>
    public class TransactionalCommandExecutorWrapper : ICommandExecutor
    {
        /// <summary>
        /// The executor to use to execute the command.
        /// </summary>
        private readonly ICommandExecutor _executor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionalCommandExecutorWrapper"/> class.
        /// </summary>
        /// <param name="executor">The executor to use to execute the command.</param>
        public TransactionalCommandExecutorWrapper(ICommandExecutor executor)
        {
            Contract.Requires<ArgumentNullException>(executor != null, "The executor cannot be null.");

            _executor = executor;
        }

        /// <summary>
        /// Executes the command within a transaction. The transaction logic uses TransactionScope.
        /// </summary>
        /// <param name="command">The command to execute. This should not be null.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>command</i> is null.</exception>
        public void Execute(ICommand command)
        {
            using (var transaction = new TransactionScope())
            {
                _executor.Execute(command);
            }
        }
    }
}

using System;
using System.Diagnostics.Contracts;
using System.Transactions;

namespace Ncqrs.Commanding.CommandExecution
{
    /// <summary>
    /// Wraps transactional behavior around the execution of command executor.
    /// </summary>
    /// <remarks>
    /// The transaction logic uses <c>TransactionScope</c> of the .NET framework.
    /// </remarks>
    public class TransactionalCommandExecutorWrapper<TCommand> : ICommandExecutor<TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// The executor to use to execute the command.
        /// </summary>
        private readonly ICommandExecutor<TCommand> _executor;

        /// <summary>
        /// The transaction scope that will be passed to every transaction creation.
        /// </summary>
        private readonly Func<TransactionScope> _creationCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionalCommandExecutorWrapper{TCommand}"/> class.
        /// </summary>
        /// <param name="executor">The executor to use to execute the command.</param>
        public TransactionalCommandExecutorWrapper(ICommandExecutor<TCommand> executor) : this(executor, null)
        {
            Contract.Requires<ArgumentNullException>(executor != null, "The executor cannot be null.");

            _executor = executor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionalCommandExecutorWrapper{TCommand}"/> class.
        /// </summary>
        /// <param name="executor">The executor to use to execute the command.</param>
        /// <param name="scope">The <see cref="TransactionScope"/> to use with the transaction.</param>
        public TransactionalCommandExecutorWrapper(ICommandExecutor<TCommand> executor, Func<TransactionScope> transactionCreationCallback)
        {
            Contract.Requires<ArgumentNullException>(executor != null, "The executor cannot be null.");

            _executor = executor;
            _creationCallback = transactionCreationCallback ?? CreateTransactionScope;
        }

        /// <summary>
        /// Creates a default transaction scope in respect of this article: <see cref="http://blogs.msdn.com/b/dbrowne/archive/2010/05/21/using-new-transactionscope-considered-harmful.aspx"/>.
        /// </summary>
        /// <returns></returns>
        private static TransactionScope CreateTransactionScope()
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.MaximumTimeout
            };
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);   
        }

        /// <summary>
        /// Executes the command within a transaction. The transaction logic uses TransactionScope.
        /// </summary>
        /// <param name="command">The command to execute. This should not be null.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>command</i> is null.</exception>
        public void Execute(TCommand command)
        {
            using (var transaction = _creationCallback())
            {
                _executor.Execute(command);
                transaction.Complete();
            }
        }
    }
}

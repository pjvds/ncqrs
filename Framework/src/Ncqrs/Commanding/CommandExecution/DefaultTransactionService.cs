using System;
using System.Transactions;

namespace Ncqrs.Commanding.CommandExecution
{
    public class DefaultTransactionService : ITransactionService
    {
        private readonly TransactionOptions _options;        

        public DefaultTransactionService()
        {
            ScopeOption = TransactionScopeOption.Required;
            _options = new TransactionOptions
                           {
                               IsolationLevel = IsolationLevel.ReadCommitted, 
                               Timeout = TransactionManager.MaximumTimeout
                           };
        }

        public DefaultTransactionService(TransactionOptions options)
        {
            ScopeOption = TransactionScopeOption.Required;
            _options = options;
        }


        public TransactionOptions Options
        {
            get { return _options; }
        }

        public TransactionScopeOption ScopeOption { get; set; }

        public void ExecuteInTransaction(Action action)
        {
            using (var scope = new TransactionScope(ScopeOption, Options))
            {
                action();
                scope.Complete();
            }
        }
    }
}
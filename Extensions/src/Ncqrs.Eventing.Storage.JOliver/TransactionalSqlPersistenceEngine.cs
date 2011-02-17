using System.Transactions;
using EventStore.Persistence.SqlPersistence;
using EventStore.Serialization;

namespace Ncqrs.Eventing.Storage.JOliver
{
    public class TransactionalSqlPersistenceEngine : SqlPersistenceEngine
    {
        public TransactionalSqlPersistenceEngine(IConnectionFactory connectionFactory, ISqlDialect dialect, ISerialize serializer) : base(connectionFactory, dialect, serializer)
        {
        }

        protected override TransactionScope OpenCommandScope()
        {
            return new TransactionScope(TransactionScopeOption.Required, GetTransactionOptions());
        }
        
        protected override TransactionScope OpenQueryScope()
        {
            return new TransactionScope(TransactionScopeOption.Suppress);
        }

        private static TransactionOptions GetTransactionOptions()
        {
            return new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.MaximumTimeout
            };
        }

    }
}
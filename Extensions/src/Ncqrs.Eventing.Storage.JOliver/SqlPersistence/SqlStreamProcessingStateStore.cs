using System;
using System.Data;
using System.Linq;
using System.Transactions;
using EventStore.Persistence.SqlPersistence;

namespace Ncqrs.Eventing.Storage.JOliver.SqlPersistence
{
    public class SqlStreamProcessingStateStore : IStreamProcessingStateStore
    {
        private readonly string _pipelineName;
        private readonly IStreamProcessingStateStoreConnectionFactory _connectionFactory;
        private readonly IStreamProcessingStateStoreSqlDialect _dialect;

        public SqlStreamProcessingStateStore(
            string pipelineName,
            IStreamProcessingStateStoreConnectionFactory connectionFactory, 
            IStreamProcessingStateStoreSqlDialect dialect)
        {
            _pipelineName = pipelineName;
            _connectionFactory = connectionFactory;
            _dialect = dialect;
        }

        public DateTime? GetLastProcessedCommitTimestamp()
        {
            return Execute(x =>
                               {
                                   x.AddParameter(_dialect.PipelineName, _pipelineName);
                                   return x.ExecuteWithQuery(_dialect.GetLastProcessedCommitTimestamp,
                                                      r => ToDateTime(r["Value"])).SingleOrDefault();
                               });
        }

        public void MarkLastProcessedCommitTimestamp(DateTime timestamp)
        {
            Execute(x =>
            {
                x.AddParameter(_dialect.PipelineName, _pipelineName);
                x.AddParameter(_dialect.CommitTimestamp, timestamp);
                return x.Execute(_dialect.MarkLastProcessedCommitTimestamp);
            });
        }

        public T Execute<T>(Func<IDbStatement, T> command)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
            using (var connection = _connectionFactory.OpenConnection())
            using (var transaction = _dialect.OpenTransaction(connection))
            using (var statement = _dialect.BuildStatement(connection, transaction, scope))
            {
                T result = command(statement);
                if (transaction != null)
                {
                    transaction.Commit();
                }
                return result;
            }
        }

        private static DateTime? ToDateTime(object value)
        {
            value = value is decimal ? (long)(decimal)value : value;
            return value is long ? new DateTime((long)value) : (DateTime)value;
        }
    }

    public interface IStreamProcessingStateStoreConnectionFactory
    {
        IDbConnection OpenConnection();
    }
}
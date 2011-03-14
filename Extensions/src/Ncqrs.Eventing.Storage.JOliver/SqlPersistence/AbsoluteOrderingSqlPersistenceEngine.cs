using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using EventStore;
using EventStore.Persistence.SqlPersistence;
using EventStore.Serialization;

namespace Ncqrs.Eventing.Storage.JOliver.SqlPersistence
{
    public class AbsoluteOrderingSqlPersistenceEngine : SqlPersistenceEngine, IPersistStreamsWithAbsoluteOrdering
    {
        private readonly IPipelineStoreSqlDialect _dialect;
        private readonly ISerialize _serializer;
        private readonly bool _transactional;

        public AbsoluteOrderingSqlPersistenceEngine(IConnectionFactory connectionFactory, 
            ISqlDialect dialect, 
            IPipelineStoreSqlDialect pipelineStoreSqlDialect,
            ISerialize serializer, bool transactional)
            : base(connectionFactory, dialect, serializer)
        {
            _dialect = pipelineStoreSqlDialect;
            _transactional = transactional;
            _serializer = serializer;
        }

        public override void Initialize()
        {
            base.Initialize();
            ExecuteCommand(Guid.Empty, statement =>
                statement.ExecuteWithSuppression(_dialect.Initialize));
        }

        public override void Commit(Commit attempt)
        {
            AppendToSequence(attempt);
            base.Commit(attempt);
        }

        private void AppendToSequence(Commit attempt)
        {
            ExecuteCommand(attempt.StreamId,
                           command =>
                               {
                                   command.AddParameter(_dialect.CommitId, attempt.CommitId);
                                   command.Execute(_dialect.RegisterSequentialId);
                               });
        }

        public IEnumerable<Commit> Fetch(long mostRecentSequentialId, int maxCount)
        {
            return ExecuteQuery(Guid.Empty,
                                query =>
                                    {
                                        var statement = _dialect.GetCommitsAfter;
                                        query.AddParameter(_dialect.SequentialId, mostRecentSequentialId);
                                        return query.ExecuteWithQuery(statement, GetCommit);
                                    }).Take(maxCount);
        }

        private Commit GetCommit(IDataRecord x)
        {
            var commit = x.GetCommit(_serializer);
            commit.Headers[_dialect.SequentialIdColumn] = x[_dialect.SequentialIdColumn];
            return commit;
        }

        public long GetLastProcessedSequentialNumber(string pipelineName)
        {
            return ExecuteQuery(Guid.Empty,
                           query =>
                           {
                               query.AddParameter(_dialect.PipelineName, pipelineName);
                               return query.ExecuteWithQuery(_dialect.GetLastProcessedCommit, x => (long)x[_dialect.SequentialIdColumn]).FirstOrDefault();
                           });
        }

        public void MarkLastProcessed(string pipelineName, Guid lastProcessedCommitSource, Guid lastProcessedCommitId)
        {
            ExecuteCommand(lastProcessedCommitSource,
                           command =>
                               {
                                   command.AddParameter(_dialect.PipelineName, pipelineName);
                                   command.AddParameter(_dialect.CommitId, lastProcessedCommitId);
                                   command.Execute(_dialect.MarkLastProcessedCommit);
                               });
        }

        protected override TransactionScope OpenCommandScope()
        {
            return _transactional 
                ? new TransactionScope(TransactionScopeOption.Required, GetTransactionOptions()) 
                : new TransactionScope(TransactionScopeOption.Suppress);
        }

        protected override TransactionScope OpenQueryScope()
        {
            return new TransactionScope(TransactionScopeOption.Suppress);
        }

        private static TransactionOptions GetTransactionOptions()
        {
            return new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.MaximumTimeout
            };
        }
    }
}
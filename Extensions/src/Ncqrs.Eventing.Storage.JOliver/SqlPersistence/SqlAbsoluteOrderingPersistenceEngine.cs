using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using EventStore;
using EventStore.Persistence.SqlPersistence;
using EventStore.Serialization;

namespace Ncqrs.Eventing.Storage.JOliver.SqlPersistence
{
    public class AbsoluteOrderingSqlPersistenceEngine : SqlPersistenceEngine, IPersistStreamsWithAbsoluteOrdering
    {
        private readonly IPipelineStoreSqlDialect _dialect;
        private readonly ISerialize _serializer;

        public AbsoluteOrderingSqlPersistenceEngine(IConnectionFactory connectionFactory, 
            ISqlDialect dialect, 
            IPipelineStoreSqlDialect pipelineStoreSqlDialect,
            ISerialize serializer)
            : base(connectionFactory, dialect, serializer)
        {
            _dialect = pipelineStoreSqlDialect;
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
            ExecuteCommand(attempt.StreamId,
                           command =>
                           {
                               command.AddParameter(_dialect.CommitId, attempt.CommitId);
                               command.Execute(_dialect.RegisterSequentialId);
                           });
            base.Commit(attempt);
        }

        public IEnumerable<Commit> Fetch(long mostRecentSequentialId)
        {
            return ExecuteQuery(Guid.Empty,
                                query =>
                                    {
                                        var statement = _dialect.GetCommitsAfter;
                                        query.AddParameter(_dialect.SequentialId, mostRecentSequentialId);
                                        return query.ExecuteWithQuery(statement, GetCommit);
                                    });
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
    }
}
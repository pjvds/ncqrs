using System;
using System.Data;
using EventStore.Persistence.SqlPersistence;

namespace Ncqrs.Eventing.Storage.JOliver.SqlPersistence
{
    public interface IStreamProcessingStateStoreSqlDialect
    {
        void InitializeStrorage();

        string GetLastProcessedCommitTimestamp { get; }
        string MarkLastProcessedCommitTimestamp { get; }

        string CommitTimestamp { get; }
        string PipelineName { get; }

        IDbTransaction OpenTransaction(IDbConnection connection);
        IDbStatement BuildStatement(IDbConnection connection, IDbTransaction transaction, params IDisposable[] resources);
    }
}
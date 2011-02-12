using System;
using System.Data;
using EventStore.Persistence.SqlPersistence;
using EventStore.Persistence.SqlPersistence.SqlDialects;

namespace Ncqrs.Eventing.Storage.JOliver.SqlPersistence
{
    public abstract class CommonStreamProcessingStateStoreSqlDialect : IStreamProcessingStateStoreSqlDialect
    {
        public abstract string Initialize { get; }

        public string GetLastProcessedCommitTimestamp
        {
            get { return CommonSqlStatements.GetLastProcessedCommitTimestamp; }
        }

        public string MarkLastProcessedCommitTimestamp
        {
            get { return CommonSqlStatements.MarkLastProcessedCommitTimestamp; }
        }

        public string CommitTimestamp { get { return "@CommitTimestamp"; } }
        public string PipelineName { get { return "@PipelineName"; } }

        public virtual IDbTransaction OpenTransaction(IDbConnection connection)
        {
            return null;
        }
        public virtual IDbStatement BuildStatement(IDbConnection connection, IDbTransaction transaction, params IDisposable[] resources)
        {
            return new CommonDbStatement(connection, transaction, resources);
        }
    }
}
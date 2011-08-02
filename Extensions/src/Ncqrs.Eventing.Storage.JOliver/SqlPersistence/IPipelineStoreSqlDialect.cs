using System;
using System.Data;
using EventStore.Persistence.SqlPersistence;

namespace Ncqrs.Eventing.Storage.JOliver.SqlPersistence
{
    public interface IPipelineStoreSqlDialect
    {
        string Initialize { get; }

        string GetCommitsAfter { get; }
        string MarkLastProcessedCommit { get; }
        string GetLastProcessedCommit { get; }
        string RegisterSequentialId { get; }

        string PipelineName { get; }
        string CommitId { get; }
        string SequentialIdColumn { get; }
        string SequentialId { get; }
    }
}
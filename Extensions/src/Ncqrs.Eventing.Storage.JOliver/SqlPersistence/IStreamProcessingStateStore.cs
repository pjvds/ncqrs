using System;

namespace Ncqrs.Eventing.Storage.JOliver.SqlPersistence
{
    public interface IStreamProcessingStateStore
    {
        DateTime GetLastProcessedCommitTimestamp(string pipelineName);
        void MarkLastProcessedCommitTimestamp(string pipelineName, DateTime timestamp);
        void Initialize();
    }
}
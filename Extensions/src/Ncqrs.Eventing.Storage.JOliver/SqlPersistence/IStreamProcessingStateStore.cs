using System;

namespace Ncqrs.Eventing.Storage.JOliver.SqlPersistence
{
    public interface IStreamProcessingStateStore
    {
        DateTime GetLastProcessedCommitTimestamp();
        void MarkLastProcessedCommitTimestamp(DateTime timestamp);
    }
}
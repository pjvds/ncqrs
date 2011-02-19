using System;
using System.Collections.Generic;
using EventStore;
using EventStore.Persistence;

namespace Ncqrs.Eventing.Storage.JOliver
{
    public interface IPersistStreamsWithAbsoluteOrdering : IPersistStreams
    {
        IEnumerable<Commit> Fetch(long mostRecentSequentialId, int maxCount);
        long GetLastProcessedSequentialNumber(string pipelineName);
        void MarkLastProcessed(string pipelineName, Guid lastProcessedCommitSource, Guid lastProcessedCommitId);
    }
}
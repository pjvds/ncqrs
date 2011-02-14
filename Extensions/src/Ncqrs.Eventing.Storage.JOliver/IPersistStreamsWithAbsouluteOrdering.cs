using System;
using System.Collections.Generic;
using EventStore;
using EventStore.Persistence;

namespace Ncqrs.Eventing.Storage.JOliver
{
    public interface IPersistStreamsWithAbsouluteOrdering : IPersistStreams
    {
        IEnumerable<Commit> Fetch(long mostRecentSequentialId);
        long GetLastProcessedSequentialNumber(string pipelineName);
        void MarkLastProcessed(string pipelineName, Guid lastProcessedCommitSource, Guid lastProcessedCommitId);
    }
}
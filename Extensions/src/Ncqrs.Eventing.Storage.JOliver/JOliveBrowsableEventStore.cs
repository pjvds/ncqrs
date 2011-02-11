using System;
using System.Collections.Generic;
using System.Linq;
using EventStore;
using EventStore.Persistence;
using Ncqrs.EventBus;
using Ncqrs.Eventing.Storage.JOliver.SqlPersistence;

namespace Ncqrs.Eventing.Storage.JOliver
{
    public class JOliveBrowsableEventStore : IBrowsableElementStore
    {
        private readonly IStreamProcessingStateStore _stateStore;
        private readonly IPersistStreams _streamStore;
        private DateTime? _lastCommitTimestamp;

        public JOliveBrowsableEventStore(IPersistStreams streamStore, IStreamProcessingStateStore stateStore)
        {
            _streamStore = streamStore;
            _stateStore = stateStore;
        }

        public IEnumerable<IProcessingElement> Fetch(int maxCount)
        {
            if (!_lastCommitTimestamp.HasValue)
            {
                _lastCommitTimestamp = _stateStore.GetLastProcessedCommitTimestamp();
            }
            var commits = _streamStore.GetFrom(_lastCommitTimestamp.Value).Take(maxCount);
            foreach (var commit in commits)
            {
                _lastCommitTimestamp = commit.CommitStamp;
                yield return new CommitProcessingElement(commit);
            }
        }

        public void MarkLastProcessedElement(IProcessingElement processingElement)
        {
            var typedElement = (CommitProcessingElement) processingElement;
            _stateStore.MarkLastProcessedCommitTimestamp(typedElement.Commit.CommitStamp);
        }
    }
}
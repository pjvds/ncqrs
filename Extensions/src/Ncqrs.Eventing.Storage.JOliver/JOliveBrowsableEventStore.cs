using System;
using System.Collections.Generic;
using System.Linq;
using EventStore.Persistence;
using Ncqrs.EventBus;
using Ncqrs.Eventing.Storage.JOliver.SqlPersistence;

namespace Ncqrs.Eventing.Storage.JOliver
{
    public class JOliveBrowsableEventStore : IBrowsableElementStore
    {
        private readonly IStreamProcessingStateStore _stateStore;
        private readonly IPersistStreams _streamStore;
        private DateTime _lastCommitTimestamp = DateTime.MinValue;
        private readonly TimeSpan _overlap;

        public JOliveBrowsableEventStore(IPersistStreams streamStore, IStreamProcessingStateStore stateStore, TimeSpan overlap)
        {
            _streamStore = streamStore;
            _overlap = overlap;
            _stateStore = stateStore;
        }

        public IEnumerable<IProcessingElement> Fetch(int maxCount)
        {
            if (_lastCommitTimestamp != DateTime.MinValue)
            {
                _lastCommitTimestamp = _stateStore.GetLastProcessedCommitTimestamp();
            }
            var commits = _streamStore.GetFrom(GetLastCommitTimestamp()).Take(maxCount);
            foreach (var commit in commits)
            {
                _lastCommitTimestamp = commit.CommitStamp;
                yield return new CommitProcessingElement(commit);
            }
        }

        private DateTime GetLastCommitTimestamp()
        {
            return _lastCommitTimestamp != DateTime.MinValue 
                ? _lastCommitTimestamp - _overlap 
                : DateTime.MinValue;
        }

        public void MarkLastProcessedElement(IProcessingElement processingElement)
        {
            var typedElement = (CommitProcessingElement) processingElement;
            _stateStore.MarkLastProcessedCommitTimestamp(typedElement.Commit.CommitStamp);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using EventStore;
using EventStore.Persistence;
using Ncqrs.EventBus;
using Ncqrs.Eventing.Storage.JOliver.SqlPersistence;

namespace Ncqrs.Eventing.Storage.JOliver
{
    public class JoesBrowsableEventStore : IBrowsableElementStore
    {
        private readonly IStreamProcessingStateStore _stateStore;
        private readonly IPersistStreams _streamStore;
        private DateTime _lastCommitTimestamp = DateTime.MinValue;
        private readonly TimeSpan _overlap;

        public JoesBrowsableEventStore(IPersistStreams streamStore, IStreamProcessingStateStore stateStore, TimeSpan overlap)
        {
            _streamStore = streamStore;
            _overlap = overlap;
            _stateStore = stateStore;
            _stateStore.Initialize();
        }        

        public IEnumerable<IProcessingElement> Fetch(string pipelineName, int maxCount)
        {
            if (_lastCommitTimestamp == DateTime.MinValue)
            {
                _lastCommitTimestamp = _stateStore.GetLastProcessedCommitTimestamp(pipelineName);
            }
            var commits = _streamStore.GetFrom(GetLastCommitTimestamp()).Take(maxCount);
            foreach (var commit in commits)
            {
                _lastCommitTimestamp = commit.CommitStamp;
                foreach (EventMessage eventMessage in commit.Events)
                {
                    var storedEvent = (StoredEvent) eventMessage.Body;
                    yield return new SourcedEventProcessingElement(storedEvent.Convert(commit.StreamId));
                }
            }
        }

        private DateTime GetLastCommitTimestamp()
        {
            return _lastCommitTimestamp != DateTime.MinValue 
                ? _lastCommitTimestamp - _overlap 
                : DateTime.MinValue;
        }

        public void MarkLastProcessedElement(string pipelineName, IProcessingElement processingElement)
        {
            var typedElement = (CommitProcessingElement) processingElement;
            _stateStore.MarkLastProcessedCommitTimestamp(pipelineName, typedElement.Commit.CommitStamp);
        }
    }
}
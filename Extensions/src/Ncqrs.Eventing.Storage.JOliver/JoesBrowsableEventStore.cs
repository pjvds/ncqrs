using System;
using System.Collections.Generic;
using System.Linq;
using EventStore;
using EventStore.Persistence;
using Ncqrs.EventBus;

namespace Ncqrs.Eventing.Storage.JOliver
{
    public class JoesBrowsableEventStore : IBrowsableElementStore
    {
        private const int EmtpySequentialIdValue = 0;

        private readonly IPersistStreamsWithAbsouluteOrdering _streamStore;
        private long _lastCommitSequentialId = EmtpySequentialIdValue;
        private bool _firstCommitFetched;

        public JoesBrowsableEventStore(IPersistStreams streamStore)
        {
            if (!(streamStore is IPersistStreamsWithAbsouluteOrdering))
            {
                throw new ArgumentException("The stream store must impement IPersistStreamsWithAbsouluteOrdering in order to be used with JoesBrowsableEventStore", "streamStore");
            }
            _streamStore = (IPersistStreamsWithAbsouluteOrdering)streamStore;
        }        

        public IEnumerable<IProcessingElement> Fetch(string pipelineName, int maxCount)
        {
            if (_lastCommitSequentialId == EmtpySequentialIdValue)
            {
                _lastCommitSequentialId = _streamStore.GetLastProcessedSequentialNumber(pipelineName);
            }
            var commits = _streamStore.Fetch(_lastCommitSequentialId).Take(maxCount);
            foreach (var commit in commits)
            {
                var thisCommitSequentialId = (long)commit.Headers["SequentialId"];
                if (_firstCommitFetched && _lastCommitSequentialId == thisCommitSequentialId)
                {
                    continue;
                }
                _lastCommitSequentialId = thisCommitSequentialId;
                foreach (EventMessage eventMessage in commit.Events)
                {
                    var storedEvent = (StoredEvent) eventMessage.Body;
                    yield return new SourcedEventProcessingElement(storedEvent.Convert(commit.StreamId));
                }
                _firstCommitFetched = true;
            }
        }

        public void MarkLastProcessedElement(string pipelineName, IProcessingElement processingElement)
        {
            var typedElement = (SourcedEventProcessingElement)processingElement;
            _streamStore.MarkLastProcessed(pipelineName, typedElement.Event.EventSourceId, typedElement.Event.CommitId);
        }
    }
}
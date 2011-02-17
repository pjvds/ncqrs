using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore;
using EventStore.Dispatcher;
using EventStore.Persistence;
using Snapshot = Ncqrs.Eventing.Sourcing.Snapshotting.Snapshot;

namespace Ncqrs.Eventing.Storage.JOliver
{
    public class JoesEventStoreAdapter : IEventStore, ISnapshotStore
    {
        private readonly ICommitEvents _eventCommitter;
        private readonly IAccessSnapshots _snapshotAccessor;

        public JoesEventStoreAdapter(IPersistStreams streamPersister)
        {
            if (!(streamPersister is IPersistStreamsWithAbsoluteOrdering))
            {
                throw new ArgumentException("The stream store must impement IPersistStreamsWithAbsoluteOrdering in order to be used with JoesEventStoreAdapter", "streamStore");
            }
            var store = new OptimisticEventStore(streamPersister, new NullDispatcher());
            _eventCommitter = store;
            _snapshotAccessor = store;
        }

        public CommittedEventStream ReadFrom(Guid id, long minVersion, long maxVersion)
        {
            int maxRevision = maxVersion == long.MaxValue ? int.MaxValue : (int) maxVersion;
            int minRevision = minVersion == long.MinValue ? int.MinValue : (int) minVersion;

            var committedEvents = _eventCommitter.GetFrom(id, minRevision, maxRevision)
                .SelectMany(x => x.Events)
                .Select(x => x.Body)
                .Cast<StoredEvent>()
                .Select(x => x.Convert(id));
            return new CommittedEventStream(id, committedEvents);
        }

        public void Store(UncommittedEventStream eventStream)
        {
            if (!eventStream.HasSingleSource)
            {
                throw new NotSupportedException("This event store don't support events streams with multiple sources.");
            }

            foreach (EventSourceInformation source in eventStream.Sources)
            {
                StoreSingleSource(source, eventStream);
            }
        }

        private void StoreSingleSource(EventSourceInformation sourceInformation, UncommittedEventStream eventStream)
        {
            var events = eventStream
                .Where(x => x.EventSourceId == sourceInformation.Id)
                .Select(x => new EventMessage
                                 {
                                     Body = new StoredEvent
                                                {
                                                    Body = x.Payload,
                                                    CommitId = eventStream.CommitId,
                                                    EventId = x.EventIdentifier,
                                                    MajorVersion = x.EventVersion.Major,
                                                    MinorVersion = x.EventVersion.Minor,
                                                    Sequence = x.EventSequence,
                                                    TimeStamp = x.EventTimeStamp
                                                }
                                 }).ToList();

            var commitAttempt = new Commit(eventStream.SourceId,
                                           (int) sourceInformation.CurrentVersion,
                                           eventStream.CommitId,
                                           (int) sourceInformation.CurrentVersion,
                                           DateTime.UtcNow,
                                           new Dictionary<string, object>(),
                                           events);
            
            _eventCommitter.Commit(commitAttempt);
        }

        private class NullDispatcher : IDispatchCommits
        {
            public void Dispose()
            {                
            }

            public void Dispatch(Commit commit)
            {             
            }
        }

        public void SaveShapshot(Snapshot snapshot)
        {
            _snapshotAccessor.AddSnapshot(new EventStore.Snapshot(snapshot.EventSourceId, (int) snapshot.Version,
                                                                  snapshot.Payload));
        }

        public Snapshot GetSnapshot(Guid eventSourceId, long maxVersion)
        {
            int maxRevision = maxVersion == long.MaxValue ? int.MaxValue : (int) maxVersion;
            var result = _snapshotAccessor.GetSnapshot(eventSourceId, maxRevision);
            if (result != null)
            {
                return new Snapshot(result.StreamId, result.StreamRevision, result.Payload);
            }
            return null;
        }
    }
}

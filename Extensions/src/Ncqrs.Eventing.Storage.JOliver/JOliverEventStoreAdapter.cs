using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore;
using EventStore.Dispatcher;
using EventStore.Persistence;

namespace Ncqrs.Eventing.Storage.JOliver
{
    public class JOliverEventStoreAdapter : IEventStore
    {
        private readonly IStoreEvents _wrappedEventStore;

        public JOliverEventStoreAdapter(IStoreEvents delegatedEventStore)
        {
            _wrappedEventStore = delegatedEventStore;
        }

        public JOliverEventStoreAdapter(IPersistStreams streamPersister)
        {
            _wrappedEventStore = new OptimisticEventStore(streamPersister, new NullDispatcher());
        }

        public CommittedEventStream ReadUntil(Guid id, long? maxVersion)
        {
            var maxRevision = maxVersion ?? int.MaxValue;
            var originalStream = _wrappedEventStore.ReadUntil(id, (int)maxRevision);
            return new CommittedEventStream(originalStream.Events.Cast<StoredEvent>().Select(x => Convert(x, id)));
        }

        private static CommittedEvent Convert(StoredEvent x, Guid id)
        {
            return new CommittedEvent(x.CommitId, x.EventId, id, x.Sequence, x.TimeStamp, x.Body,
                                      new Version(x.MajorVersion, x.MinorVersion));
        }

        public CommittedEventStream ReadFrom(Guid id, long minVersion)
        {
            var originalStream = _wrappedEventStore.ReadFrom(id, (int)minVersion);
            return new CommittedEventStream(originalStream.Events.Cast<StoredEvent>().Select(x => Convert(x, id)));
        }

        public void Store(UncommittedEventStream eventStream)
        {
            if (!eventStream.HasSingleSource)
            {
                throw new NotSupportedException("This event store don't support events streams with multiple sources.");
            }
            var sourceInformation = eventStream.Sources.Single();
            var commitAttempt = new CommitAttempt
                                    {
                                        CommitId = eventStream.CommitId,
                                        PreviousCommitSequence = (int) sourceInformation.InitialVersion,
                                        StreamId = eventStream.SourceId,
                                        StreamRevision = (int)sourceInformation.CurrentVersion,                                        
                                    };
            commitAttempt.Events.AddRange(eventStream.Select(x => new EventMessage
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
                                                                      }));
            _wrappedEventStore.Write(commitAttempt);
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
    }
}

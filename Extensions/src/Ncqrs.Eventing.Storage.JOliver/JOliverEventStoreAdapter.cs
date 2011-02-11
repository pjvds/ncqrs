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
        private readonly OptimisticEventStore _wrappedEventStore;

        public JOliverEventStoreAdapter(IPersistStreams streamPersister)
        {
            _wrappedEventStore = new OptimisticEventStore(streamPersister, new NullDispatcher());
        }
        
        private static CommittedEvent Convert(StoredEvent x, Guid id)
        {
            return new CommittedEvent(x.CommitId, x.EventId, id, x.Sequence, x.TimeStamp, x.Body,
                                      new Version(x.MajorVersion, x.MinorVersion));
        }

        public CommittedEventStream ReadFrom(Guid id, long minVersion, long maxVersion)
        {
            int maxRevision = maxVersion == long.MaxValue ? int.MaxValue : (int) maxVersion;
            int minRevision = minVersion == long.MinValue ? int.MinValue : (int) minVersion;

            var committedEvents = _wrappedEventStore.GetFrom(id, minRevision, maxRevision)
                .SelectMany(x => x.Events)
                .Select(x => x.Body)
                .Cast<StoredEvent>()
                .Select(x => Convert(x, id));
            return new CommittedEventStream(committedEvents);
        }

        public void Store(UncommittedEventStream eventStream)
        {
            if (!eventStream.HasSingleSource)
            {
                throw new NotSupportedException("This event store don't support events streams with multiple sources.");
            }
            
            var sourceInformation = eventStream.Sources.Single();

            var events = eventStream.Select(x => new EventMessage
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

            var commitAttempt = new Commit(eventStream.SourceId, (int)sourceInformation.CurrentVersion,
                                    eventStream.CommitId,
                                    (int)sourceInformation.InitialVersion,
                                    DateTime.UtcNow,
                                    new Dictionary<string, object>(),
                                    events);
            
            _wrappedEventStore.Commit(commitAttempt);
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

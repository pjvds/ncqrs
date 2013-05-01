using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using EventStore;
using Ncqrs.Domain;
using Ncqrs.Domain.Storage;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Sourcing.Snapshotting;

namespace Ncqrs.Eventing.Storage.JOliver
{
    public class JoesUnitOfWork : UnitOfWorkBase
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Guid _commitId;
        private readonly List<AggregateRoot> _dirtyInstances = new List<AggregateRoot>();
        private readonly IStoreEvents _eventStore;
        private readonly IDomainRepository _domainRepository;
        private readonly ISnapshotStore _snapshotStore;
        private readonly IEventBus _eventBus;
        private readonly ISnapshottingPolicy _snapshottingPolicy;
        private readonly Dictionary<Guid, IEventStream> _trackedStreams = new Dictionary<Guid, IEventStream>();
        private readonly UncommittedEventStream _eventStream;

        public JoesUnitOfWork(Guid commandId, IDomainRepository domainRepository, IStoreEvents eventStore, ISnapshotStore snapshotStore, IEventBus eventBus, ISnapshottingPolicy snapshottingPolicy)
            : base(commandId)
        {
            _eventStream = new UncommittedEventStream(commandId);
            _commitId = commandId;
            _eventStore = eventStore;
            _domainRepository = domainRepository;
            _snapshotStore = snapshotStore;
            _eventBus = eventBus;
            _snapshottingPolicy = snapshottingPolicy;
        }

        protected override void AggregateRootEventAppliedHandler(AggregateRoot aggregateRoot, UncommittedEvent evnt)
        {
            var message = new EventMessage
                              {
                                  Body = new StoredEvent
                                             {
                                                 Body = evnt.Payload,
                                                 CommitId = _commitId,
                                                 EventId = evnt.EventIdentifier,
                                                 MajorVersion = evnt.EventVersion.Major,
                                                 MinorVersion = evnt.EventVersion.Minor,
                                                 Sequence = evnt.EventSequence,
                                                 TimeStamp = evnt.EventTimeStamp
                                             }
                              };
            IEventStream stream;
            var id = aggregateRoot.EventSourceId;
            if (!_trackedStreams.TryGetValue(id, out stream))
            {
                stream = _eventStore.CreateStream(id);
                _trackedStreams[id] = stream;
            }
            stream.Add(message);
            _eventStream.Append(evnt);
            _dirtyInstances.Add(aggregateRoot);
        }

        public override AggregateRoot GetById(Type aggregateRootType, Guid eventSourceId, long? lastKnownRevision)
        {
            int maxRevision = lastKnownRevision != null ? (int) lastKnownRevision : int.MaxValue;
            var snapshot = _eventStore.GetSnapshot(eventSourceId, maxRevision);
            IEventStream stream;
            Sourcing.Snapshotting.Snapshot ncqrsSnapshot = null;
            if (snapshot != null)
            {
                ncqrsSnapshot = new Sourcing.Snapshotting.Snapshot(eventSourceId, snapshot.StreamRevision, snapshot.Payload);
                stream = _eventStore.OpenStream(snapshot, maxRevision);
            }
            else
            {
                stream = _eventStore.OpenStream(eventSourceId, 0, maxRevision);
            }
            _trackedStreams[eventSourceId] = stream;
            var committedEventStream = GetCommittedEventStream(eventSourceId, stream);
            var result = _domainRepository.Load(aggregateRootType, ncqrsSnapshot, committedEventStream);
            return result;
        }

        private static CommittedEventStream GetCommittedEventStream(Guid eventSourceId, IEventStream stream)
        {
            var committedEvents = stream.CommittedEvents
                .Select(x => x.Body)
                .Cast<StoredEvent>()
                .Select(x => x.Convert(eventSourceId));
            return new CommittedEventStream(eventSourceId, committedEvents);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            foreach (var trackedStream in _trackedStreams.Values)
            {
                trackedStream.Dispose();
            }
        }

        public override void Accept()
        {
            Log.DebugFormat("Accepting unit of work {0}", this);
            foreach (IEventStream trackedStream in _trackedStreams.Values)
            {
                trackedStream.CommitChanges(_commitId);
                trackedStream.Dispose();
            }
            _trackedStreams.Clear();
            Log.DebugFormat("Storing the event stream for command {0} to event store", _commitId);
            Log.DebugFormat("Publishing events for command {0} to event bus", _commitId);
            _eventBus.Publish(_eventStream);
            CreateSnapshots();
        }

        private void CreateSnapshots()
        {
            foreach (AggregateRoot savedInstance in _dirtyInstances)
            {
                TryCreateCreateSnapshot(savedInstance);
            }
        }

        private void TryCreateCreateSnapshot(AggregateRoot savedInstance)
        {
            if (_snapshottingPolicy.ShouldCreateSnapshot(savedInstance))
            {
                var snapshot = _domainRepository.TryTakeSnapshot(savedInstance);
                if (snapshot != null)
                {
                    _snapshotStore.SaveSnapshot(snapshot);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0}@{1}", _commitId, Thread.CurrentThread.ManagedThreadId);
        }
    }
}
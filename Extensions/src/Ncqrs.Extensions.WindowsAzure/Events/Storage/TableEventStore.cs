using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Storage;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Data.Services.Client;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Sourcing.Snapshotting;

namespace Ncqrs.Extensions.WindowsAzure.Events.Storage {
    /// <summary>
    /// An event store. Can store and load events from an <see cref="IEventSource"/>.
    /// </summary>
    /// <remarks>Implemented using Windows Azure Table Storage</remarks>
    public class TableEventStore : IEventStore {
        private CloudStorageAccount _account = null;
        private string _tableName = "NcqrsEvents";
        private string _blobContainer = "NcqrsSnapshots".ToLowerInvariant();

        public TableEventStore(CloudStorageAccount account) : this(account, null) {
        }

        public TableEventStore(CloudStorageAccount account, string tablePrefix) {
            _account = account;
            _tableName = tablePrefix + _tableName;
        }
        private static IList<string> _createdTables = new List<string>();
        /// <summary>
        /// Reads from the stream from the <paramref name="minVersion"/> up until <paramref name="maxVersion"/>.
        /// </summary>
        /// <remarks>
        /// Returned event stream does not contain snapshots. This method is used when snapshots are stored in a separate store.
        /// </remarks>
        /// <param name="id">The id of the event source that owns the events.</param>
        /// <param name="minVersion">The minimum version number to be read.</param>
        /// <param name="maxVersion">The maximum version number to be read</param>
        /// <returns>All the events from the event source between specified version numbers.</returns>
        public Eventing.CommittedEventStream ReadFrom(Guid id, long minVersion, long maxVersion) {
            if (!_createdTables.Contains(_tableName)) {
                lock (_createdTables) {
                    if (!_createdTables.Contains(_tableName)) {
                        _account.CreateCloudTableClient().CreateTableIfNotExist(_tableName);
                        _createdTables.Add(_tableName);
                    }
                }
            }

            CloudTableQuery<EventEntity> eventStream = Utility.GetContext(_account, _tableName)
                .CreateQuery<EventEntity>(_tableName)
                .Where(entity => entity.PartitionKey == id.ToString()//).AsTableServiceQuery();
                    &&
                    entity.RowKey.CompareTo(Utility.GetRowKey(minVersion)) >= 0 &&
                    entity.RowKey.CompareTo(Utility.GetRowKey(maxVersion)) <= 0).AsTableServiceQuery();
            return new Eventing.CommittedEventStream(id,
                eventStream.ToList().Select(e => new Ncqrs.Eventing.CommittedEvent(
                    e.CommitId,
                    e.EventIdentifier,
                    e.EventSourceId,
                    e.EventSequence,
                    e.EventTimeStamp,
                    Utility.DeJsonize(e.Payload, e.Name),
                    System.Version.Parse(e.EventVersion))
                    )
                );
        }

        private void SaveEvents(Guid eventSourceId, IEnumerable<UncommittedEvent> events) {
            if (!_createdTables.Contains(_tableName)) {
                lock (_createdTables) {
                    if (!_createdTables.Contains(_tableName)) {
                        _account.CreateCloudTableClient().CreateTableIfNotExist(_tableName);
                        _createdTables.Add(_tableName);
                    }
                }
            }
            string eventSourceName = events.First().GetType().ToString();
            long initialVersion = events.First().InitialVersionOfEventSource;
            long lastVersion = initialVersion + events.Count();
            TableServiceContext context = Utility.GetContext(_account, _tableName);

            EventSourceEntity lastSource = context.CreateQuery<EventSourceEntity>(_tableName)
                .Where(e => e.PartitionKey == eventSourceId.ToString() &&
                    e.RowKey == "EventSource_" + eventSourceId.ToString()).AsTableServiceQuery().FirstOrDefault();

            if (lastSource == null) {
                lastSource = new EventSourceEntity(eventSourceId, initialVersion);
                context.AddObject(_tableName, lastSource);
            } else if (lastSource.Version != initialVersion) {
                throw new ConcurrencyException(eventSourceId, initialVersion);
            }
            foreach (UncommittedEvent uncommitedEvent in events) {
                context.AddObject(_tableName, new EventEntity(uncommitedEvent));
            }
            lastSource.Version = lastVersion;
            context.UpdateObject(lastSource);
            context.SaveChanges(SaveChangesOptions.Batch);

        }

        /// <summary>
        /// Persists the <paramref name="eventStream"/> in the store as a single and atomic commit.
        /// </summary>
        /// <exception cref="ConcurrencyException">Occurs when there is already a newer version of the event provider stored in the event store.</exception>
        /// <param name="eventStream">The <see cref="UncommittedEventStream"/> to commit.</param>
        public void Store(Eventing.UncommittedEventStream eventStream) {
            foreach(Guid eventSourceId in eventStream.Select(es => es.EventSourceId).Distinct()) {
                SaveEvents(eventSourceId, eventStream.Where(es => es.EventSourceId == eventSourceId));
            }
        }
    }
}

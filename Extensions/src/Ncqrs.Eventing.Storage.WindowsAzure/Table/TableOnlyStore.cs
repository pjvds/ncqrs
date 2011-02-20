using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Ncqrs.Eventing.Storage.WindowsAzure
{
    /// <summary>
    /// Initialises a new event store that uses Table Storage only.
    /// </summary>
    /// <remarks>May not be appropriate for events with large payloads</remarks>
    public class TableOnlyStore : IEventStore
    {

        private static readonly CloudStorageAccount DevelopmentStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
        private CloudStorageAccount account = null;
        private string prefix = null;

        /// <summary>
        /// Creates a new TableOnlyStore, and names the table including the supplied Prefix
        /// </summary>
        /// <param name="prefix">The prefix for the table name</param>
        /// <remarks>Useful for testing and partitioning, e.g., new TableStorage("TestRun1")</remarks>
        public TableOnlyStore(string prefix)
            : this(DevelopmentStorageAccount, prefix)
        {
        }

        public TableOnlyStore()
            : this(DevelopmentStorageAccount, null)
        {
        }

        public TableOnlyStore(CloudStorageAccount storageAccount, string prefix)
        {
            account = storageAccount;
            this.prefix = prefix;
        }



        private void SaveEvents(Guid eventSourceId,
            IEnumerable<UncommittedEvent> events)
        {
            string eventSourceName = events.First().GetType().ToString();
            long initialVersion = events.First().InitialVersionOfEventSource;
            long lastVersion = initialVersion + events.Count();

            NcqrsEventStoreContext storeContext = new NcqrsEventStoreContext(eventSourceId, account, prefix);
            Guid commitId = storeContext.BeginCommit();

            NcqrsEventSource lastSource = storeContext.LatestEventSource;
            if (lastSource == null)
            {
                lastSource = new NcqrsEventSource(eventSourceId,
                    initialVersion,
                    eventSourceName);

            }
            else if (lastSource.Version != initialVersion)
            {
                throw new ConcurrencyException(eventSourceId, initialVersion);
            }

            foreach (UncommittedEvent @event in events)
            {
                storeContext.Add(new NcqrsEvent(@event));
            }

            lastSource.Version = lastVersion;
            storeContext.SaveSource(lastSource);

            storeContext.EndCommit();
        }

        public CommittedEventStream ReadFrom(Guid id, long minVersion, long maxVersion)
        {
            NcqrsEventStoreContext eventContext = new NcqrsEventStoreContext(id, account, prefix);

            IQueryable<NcqrsEvent> storeEvents = eventContext.Events;

            // 628426 20 Feb 2011
            // Azure Table Storage Client struggles with lambdas where the rvalue is long.Min and .Max
            // so the following code added when interface changed to no longer use Nullable<long>
            if (minVersion != long.MinValue)
            {
                storeEvents = storeEvents.Where(e => e.Sequence >= minVersion);
            }
            if (maxVersion != long.MaxValue)
            {
                storeEvents = storeEvents.Where(e => e.Sequence <= maxVersion);
            }

            storeEvents = storeEvents.ToList().OrderBy(e => e.Sequence).AsQueryable();

            IEnumerable<CommittedEvent> committedEvents = storeEvents.Select(
                e => new CommittedEvent(
                        e.CommitId,
                        Guid.Parse(e.RowKey),
                        Guid.Parse(e.PartitionKey),
                        e.Sequence,
                        e.Timestamp,
                        Utility.DeJsonize(e.Data, e.Name),
                        Version.Parse(e.Version)
                        )
                );

            return new CommittedEventStream(id, committedEvents);
        }

        #region IEventStore Members

        public void Store(UncommittedEventStream eventStream)
        {
            Parallel.ForEach<Guid>(
                eventStream.Select(es => es.EventSourceId).Distinct(),
                (eventSourceId) => SaveEvents(eventSourceId, eventStream.Where(es => es.EventSourceId == eventSourceId))
            );

        }

        #endregion
    }
}

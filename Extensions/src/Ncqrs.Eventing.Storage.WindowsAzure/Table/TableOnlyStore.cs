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


        #region IEventStore Members

        public CommittedEventStream ReadUntil(Guid id, long? maxVersion)
        {
            NcqrsEventStoreContext eventContext = new NcqrsEventStoreContext(id, account, prefix);

            IQueryable<NcqrsEvent> events = eventContext.Events;
            if (maxVersion.HasValue)
            {
                events = eventContext.Events.Where(e => e.Sequence <= maxVersion.Value);
            }

            IList<NcqrsEvent> theEvents = events.ToList();

            return new CommittedEventStream(
                theEvents.
                    Select(e => new CommittedEvent(
                        e.CommitId,
                        Guid.Parse(e.RowKey),
                        Guid.Parse(e.PartitionKey),
                        e.Sequence,
                        e.Timestamp,
                        new BinaryFormatter()
                            .Deserialize(
                        new MemoryStream(e.Data)),
                        Version.Parse(e.Version)))
                );
        }

        public CommittedEventStream ReadFrom(Guid id, long minVersion)
        {
            NcqrsEventStoreContext eventContext = new NcqrsEventStoreContext(id, account, prefix);

            IQueryable<NcqrsEvent> events = eventContext.Events.Where(e => e.Sequence >= minVersion);
            
            return new CommittedEventStream(
                events.
                    Select(e => new CommittedEvent(
                        e.CommitId,
                        Guid.Parse(e.RowKey),
                        Guid.Parse(e.PartitionKey),
                        e.Sequence,
                        e.Timestamp,
                        new BinaryFormatter()
                            .Deserialize(
                        new MemoryStream(e.Data)),
                        Version.Parse(e.Version)))
                );
        }

        public void Store(UncommittedEventStream eventStream)
        {
            // TODO: lrn to use GroupBy
            Parallel.ForEach<Guid>(
                eventStream.Select(es => es.EventSourceId).Distinct(), 
                (eventSourceId) => SaveEvents(eventSourceId, eventStream.Where(es => es.EventSourceId == eventSourceId))
            );

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

        #endregion
    }
}

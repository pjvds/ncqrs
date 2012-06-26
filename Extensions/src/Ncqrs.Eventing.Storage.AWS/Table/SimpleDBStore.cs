using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleDB;

namespace Ncqrs.Eventing.Storage.AWS
{
    /// <summary>
    /// Initialises a new event store that uses Table Storage only.
    /// </summary>
    /// <remarks>May not be appropriate for events with large payloads</remarks>
    public class SimpleDBStore : IEventStore
    {
        readonly AmazonSimpleDB account = AWSClientFactory.CreateAmazonSimpleDBClient();
        private string prefix = null;

        public SimpleDBStore()
        { }

        /// <summary>
        /// Creates a new SimpleDBStore, and names the table including the supplied Prefix
        /// </summary>
        /// <param name="prefix">The prefix for the table name</param>
        /// <remarks>Useful for testing and partitioning, e.g., new TableStorage("TestRun1")</remarks>
        public SimpleDBStore(string prefix)
        {
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
                        e.EventIdentifier,
                        e.EventSourceId,
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
            Parallel.ForEach(
                eventStream.Select(es => es.EventSourceId).Distinct(),
                eventSourceId => SaveEvents(eventSourceId, eventStream.Where(es => es.EventSourceId == eventSourceId))
            );
        }

        #endregion
    }
}

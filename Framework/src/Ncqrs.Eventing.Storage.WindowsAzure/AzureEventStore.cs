using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Data.Services.Client;

namespace Ncqrs.Eventing.Storage.WindowsAzure
{
    public class AzureEventStore : IEventStore
    {
        private static readonly CloudStorageAccount DEFAULT_DEVELOPMENT_ACCOUNT =
            CloudStorageAccount.DevelopmentStorageAccount;

        private const string PROVIDERS_TABLE_NAME = "providers";
        private const string EVENTS_TABLE_NAME = "events";

        private CloudStorageAccount _account;
        private CloudTableClient _tableClient;

        public AzureEventStore()
            : this(DEFAULT_DEVELOPMENT_ACCOUNT)
        {
        }

        public AzureEventStore(CloudStorageAccount storageAccount)
        {
            _account = storageAccount;
            _tableClient = _account.CreateCloudTableClient();

            InitializeStorage();
        }

        private void InitializeStorage()
        {
            _tableClient.CreateTableIfNotExist(PROVIDERS_TABLE_NAME);
            _tableClient.CreateTableIfNotExist(EVENTS_TABLE_NAME);
        }

        /// <summary>
        /// Get all events provided by an specified event provider.
        /// </summary>
        /// <param name="eventSourceId">The id of the event source that owns the events.</param>
        /// <returns>
        /// All the events from the event source.
        /// </returns>
        public IEnumerable<ISourcedEvent> GetAllEventsForEventSource(Guid eventSourceId)
        {
            var context = _tableClient.GetDataServiceContext();
            var result = new List<ISourcedEvent>();

            var eventsFromSource =
                context.CreateQuery<SourcedEventEntity>(EVENTS_TABLE_NAME).Where(
                    e => e.PartitionKey == eventSourceId.ToString()).AsEnumerable().OrderBy(e => e.Sequence);

            foreach (var eventEntity in eventsFromSource)
            {
                var evnt = DeserializeEventEntity(eventEntity);
                result.Add(evnt);
            }

            return result;
        }

        private ISourcedEvent DeserializeEventEntity(SourcedEventEntity sourcedEventEntity)
        {
            // Create formatter that can deserialize our events.
            var formatter = new BinaryFormatter();
            var blobClient = _account.CreateCloudBlobClient();

            // Get event details.
            var blobAddress = GetBlobAddress(sourcedEventEntity);
            var blobRef = blobClient.GetBlobReference(blobAddress);
            var rawData = blobRef.DownloadByteArray();

            using (var dataStream = new MemoryStream(rawData))
            {
                // Deserialize event and return it.
                return (ISourcedEvent)formatter.Deserialize(dataStream);
            }
        }

        private string GetBlobAddress(SourcedEventEntity sourcedEventEntity)
        {
            return "events\\" + sourcedEventEntity.RowKey;
        }

        /// <summary>
        /// Save all events from a specific event provider.
        /// </summary>
        /// <exception cref="T:Ncqrs.Eventing.Storage.ConcurrencyException">Occurs when there is already a newer version of the event provider stored in the event store.</exception><param name="source">The source that should be saved.</param><requires description="source cannot be null." exception="T:System.ArgumentNullException">source != null</requires><exception cref="T:System.ArgumentNullException">source == null</exception><ensures description="Return should never be null.">Contract.Result&lt;IEnumerable&lt;IEvent&gt;&gt;() != null</ensures>
        public void Save(IEventSource source)
        {
            var context = _tableClient.GetDataServiceContext();
            var sourceInStore = GetSourceFromStore(context, source.Id);
            var uncommitedEvents = source.GetUncommittedEvents();

            if (sourceInStore == null)
            {
                sourceInStore = InsertNewEventSource(context, source);
            }
            else
            {
                if (source.Version != sourceInStore.Version)
                    throw new ConcurrencyException(source.Id, source.Version, sourceInStore.Version);
            }

            // Update version.
            sourceInStore.Version = source.Version + uncommitedEvents.Count(); // TODO: We can do this better.
            context.UpdateObject(sourceInStore);
            context.SaveChanges(SaveChangesOptions.Batch); // TODO: Validate response.

            PushEventToStore(context, source.Id, uncommitedEvents);

            // TODO: Validate response.
            var response = context.SaveChanges(SaveChangesOptions.Batch);
        }

        private EventSourceEntity InsertNewEventSource(TableServiceContext context, IEventSource source)
        {
            var sourceEntity = new EventSourceEntity();
            sourceEntity.RowKey = source.Id.ToString();
            sourceEntity.PartitionKey = source.GetType().FullName;
            sourceEntity.Version = source.GetUncommittedEvents().Count();

            context.AddObject(PROVIDERS_TABLE_NAME, sourceEntity);

            return sourceEntity;
        }

        private EventSourceEntity GetSourceFromStore(TableServiceContext context, Guid eventSourceId)
        {
            var query = from p in context.CreateQuery<EventSourceEntity>(PROVIDERS_TABLE_NAME)
                        where p.RowKey == eventSourceId.ToString()
                        select p;

            return query.FirstOrDefault();
        }

        private void PushEventToStore(TableServiceContext context, Guid eventSourceId, IEnumerable<ISourcedEvent> events)
        {
            var formatter = new BinaryFormatter();
            var blobClient = _account.CreateCloudBlobClient();

            var sequenceNumber = 1;

            foreach (var eventToPush in events)
            {
                using (var buffer = new MemoryStream())
                {
                    var eventEntity = new SourcedEventEntity();
                    eventEntity.RowKey = Guid.NewGuid().ToString();
                    eventEntity.PartitionKey = eventSourceId.ToString();

                    eventEntity.Sequence = sequenceNumber;
                    sequenceNumber++;

                    var rootContainer = blobClient.GetContainerReference("events");
                    rootContainer.CreateIfNotExist();

                    var permissions = new BlobContainerPermissions();
                    permissions.PublicAccess = BlobContainerPublicAccessType.Container;

                    rootContainer.SetPermissions(permissions);

                    var blob = rootContainer.GetBlobReference(eventEntity.RowKey);

                    blob.Metadata["sourceid"] = eventSourceId.ToString();

                    formatter.Serialize(buffer, eventToPush);
                    buffer.Seek(0, SeekOrigin.Begin);

                    blob.UploadFromStream(buffer);

                    context.AddObject(EVENTS_TABLE_NAME, eventEntity);
                }
            }
        }

        private void UpdateVersion(TableServiceContext context, IEventSource source, long newVersion)
        {
            var query = from s in context.CreateQuery<EventSourceEntity>("PROVIDERS_TABLE_NAME")
                        where s.RowKey == source.Id.ToString()
                        select s;

            var sourceInStore = query.ToList().SingleOrDefault();

            if(sourceInStore == null) throw new InvalidOperationException("Could not update event source version since the source has not been found in the store.");

            if(sourceInStore.Version != source.Version) throw new ConcurrencyException(source.Id, source.Version, sourceInStore.Version);

            sourceInStore.Version = newVersion;
            context.UpdateObject(sourceInStore);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;

namespace Ncqrs.Eventing.Storage.WindowsAzure
{
    internal class NcqrsEventStoreContext : TableServiceContext
    {
        private Guid _eventSourceID;
        private const string _TABLENAME = "NcqrsEventStore";
        private string TABLENAME
        {
            get
            {
                return _tablePrefix + _TABLENAME;
            }
        }
        private string _tablePrefix = null;

        public NcqrsEventStoreContext(Guid eventSourceId,
            CloudStorageAccount account)
            : this(eventSourceId, account, null)
        {
            _eventSourceID = eventSourceId;
        }

        public NcqrsEventStoreContext(Guid eventSourceId,
            CloudStorageAccount account,
            string tablePrefix)
            : base(account.TableEndpoint.AbsoluteUri,
                account.Credentials)
        {
            this.IgnoreResourceNotFoundException = true;
            _eventSourceID = eventSourceId;
            _tablePrefix = tablePrefix;
            account.CreateCloudTableClient().CreateTableIfNotExist(TABLENAME);
        }

        public IQueryable<NcqrsEvent> Events
        {
            get
            {
                string partitionKey = _eventSourceID.ToString();
                return CreateQuery<NcqrsEvent>(TABLENAME).Where(e => e.PartitionKey == partitionKey);
            }
        }

        public NcqrsEventSource LatestEventSource
        {
            get
            {
                string partitionKey = "Source_" + _eventSourceID.ToString();
                string rowKey = partitionKey;
                return CreateQuery<NcqrsEventSource>(TABLENAME).Where(eventSource => eventSource.PartitionKey == partitionKey && eventSource.RowKey == rowKey).ToList().FirstOrDefault();
            }
        }

        private Guid _commitId = Guid.Empty;

        public Guid BeginCommit()
        {
            if (_commitId != Guid.Empty)
            {
                throw new InvalidOperationException("Cannot BeginCommit while CommitId [" + _commitId + "] is still pending");
            }
            _commitId = Guid.NewGuid();
            return _commitId;
        }

        public void Add(NcqrsEvent @event)
        {
            if (_commitId == Guid.Empty)
            {
                throw new InvalidOperationException("Cannot Add events without beginning a commit.  Call the BeginCommit method");
            }
            @event.CommitId = _commitId;
            
            AddObject(TABLENAME, @event);
        }

        private void AddSource(NcqrsEventSource source)
        {
            if (_commitId == Guid.Empty)
            {
                throw new InvalidOperationException("Cannot Add event sources without beginning a commit.  Call the BeginCommit method");
            }
            AddObject(TABLENAME, source);
        }

        public void SaveSource(NcqrsEventSource source)
        {
            if (this.Entities.Contains(this.GetEntityDescriptor(source)))
            {
                UpdateSource(source);
            }
            else
            {
                AddSource(source);
            }
        }

        private void UpdateSource(NcqrsEventSource source)
        {
            if (_commitId == Guid.Empty)
            {
                throw new InvalidOperationException("Cannot update event sources without beginning a commit.  Call the BeginCommit method");
            }
            UpdateObject(source);
        }

        public void EndCommit()
        {
            SaveChanges();
        }
    }
}

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
        private bool _usingDevelopment = false;

        private string EVENTSOURCETABLENAME
        {
            get
            {
                // 628426 20 Feb 2011
                // Azure Storage Emulator doesn't support having our Event Store and Event Source in the same
                // table - this logic ensures they are created in seperate tables only in development
                if (_usingDevelopment)
                {
                    return _tablePrefix + _TABLENAME + "Source";
                }
                return _tablePrefix + _TABLENAME;
            }
        }
        private string EVENTTABLENAME
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

            if (account == CloudStorageAccount.DevelopmentStorageAccount)
            {
                _usingDevelopment = true;
            }

            account.CreateCloudTableClient().CreateTableIfNotExist(EVENTTABLENAME);
            account.CreateCloudTableClient().CreateTableIfNotExist(EVENTSOURCETABLENAME);
        }

        public IQueryable<NcqrsEvent> Events
        {
            get
            {
                string partitionKey = _eventSourceID.ToString();
                return CreateQuery<NcqrsEvent>(EVENTTABLENAME).Where(e => e.PartitionKey == partitionKey);
            }
        }

        public NcqrsEventSource LatestEventSource
        {
            get
            {
                string partitionKey = "Source_" + _eventSourceID.ToString();
                string rowKey = partitionKey;
                return CreateQuery<NcqrsEventSource>(EVENTSOURCETABLENAME).Where(eventSource => eventSource.PartitionKey == partitionKey && eventSource.RowKey == rowKey).ToList().FirstOrDefault();
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

            AddObject(EVENTTABLENAME, @event);
        }

        private void AddSource(NcqrsEventSource source)
        {
            if (_commitId == Guid.Empty)
            {
                throw new InvalidOperationException("Cannot Add event sources without beginning a commit.  Call the BeginCommit method");
            }
            AddObject(EVENTSOURCETABLENAME, source);
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

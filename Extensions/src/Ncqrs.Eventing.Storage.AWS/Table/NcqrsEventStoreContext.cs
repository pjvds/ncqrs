using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Attribute = Amazon.SimpleDB.Model.Attribute;

namespace Ncqrs.Eventing.Storage.AWS
{
    internal class NcqrsEventStoreContext
    {
        private readonly Guid _eventSourceID;
        private const string _domainName = "NcqrsEventStore";
        private readonly AmazonSimpleDB _account;

        private string EVENTSOURCETABLENAME
        {
            get
            {
                return _tablePrefix + _domainName + "Source";
            }
        }
        private string EVENTTABLENAME
        {
            get
            {
                return _tablePrefix + _domainName;
            }
        }

        private readonly string _tablePrefix;

        public NcqrsEventStoreContext(Guid eventSourceId,
            AmazonSimpleDB account)
            : this(eventSourceId, account, null)
        {
            _eventSourceID = eventSourceId;
        }

        public NcqrsEventStoreContext(Guid eventSourceId,
            AmazonSimpleDB account,
            string tablePrefix)
        {
            _account = account;
            _eventSourceID = eventSourceId;
            _tablePrefix = tablePrefix;

            CreateIfNotExist(EVENTTABLENAME);
            CreateIfNotExist(EVENTSOURCETABLENAME);
        }

        private void CreateIfNotExist(string domainName)
        {
            ListDomainsResponse response = _account.ListDomains(new ListDomainsRequest());

            if (response.ListDomainsResult.DomainName.Any(domain => domain == domainName)) return;
            _account.CreateDomain(new CreateDomainRequest { DomainName = domainName });
        }

        public IQueryable<NcqrsEvent> Events
        {
            get
            {
                string selectStmt = string.Format("select * from {0} where EventSourceId='{1}'", EVENTTABLENAME, _eventSourceID);
                SelectRequest selectRequestAction = new SelectRequest().WithSelectExpression(selectStmt);
                SelectResponse result = _account.Select(selectRequestAction);
                if (result.SelectResult.Item.Count > 0 &&
                    result.SelectResult.Item[0].Attribute.Count > 0)
                {
                    return result.SelectResult.Item.Select(
                        item => new NcqrsEvent
                                    {
                                        CommitId = new Guid(item.Attribute.First(a => a.Name == "CommitId").Value),
                                        Data = item.Attribute.First(a => a.Name == "Data").Value,
                                        EventIdentifier =
                                            new Guid(item.Attribute.First(a => a.Name == "EventIdentifier").Value),
                                        EventSourceId =
                                            new Guid(item.Attribute.First(a => a.Name == "EventSourceId").Value),
                                        Name = item.Attribute.First(a => a.Name == "Name").Value,
                                        Sequence =
                                            Convert.ToInt64(item.Attribute.First(a => a.Name == "Sequence").Value),
                                        Timestamp =
                                            Convert.ToDateTime(
                                                item.Attribute.First(a => a.Name == "Timestamp").Value),
                                        Version = item.Attribute.First(a => a.Name == "Version").Value
                                    }).AsQueryable();
                }
                return null;
            }
        }

        public NcqrsEventSource LatestEventSource
        {
            get
            {
                string selectStmt = string.Format("select * from {0} where EventSourceId='{1}'", EVENTSOURCETABLENAME, _eventSourceID);
                SelectRequest selectRequestAction = new SelectRequest().WithSelectExpression(selectStmt);
                SelectResponse result = _account.Select(selectRequestAction);
                if (result.SelectResult.Item.Count > 0 &&
                    result.SelectResult.Item[0].Attribute.Count > 0)
                {
                    List<Attribute> attributeCollection = result.SelectResult.Item[0].Attribute;
                    return new NcqrsEventSource
                               {
                                   EventSourceId = new Guid(attributeCollection.First(a => a.Name == "EventSourceId").Value),
                                   Name = attributeCollection.First(a => a.Name == "Name").Value,
                                   Timestamp = Convert.ToDateTime(attributeCollection.First(a => a.Name == "Timestamp").Value),
                                   Version = Convert.ToInt64(attributeCollection.First(a => a.Name == "Version").Value)
                               };
                }
                return null;
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

            List<ReplaceableAttribute> list =
                new List<ReplaceableAttribute>();
            list.Add(new ReplaceableAttribute { Name = "Name", Replace = true, Value = @event.Name });
            list.Add(new ReplaceableAttribute { Name = "CommitId", Replace = true, Value = @event.CommitId.ToString() });
            list.Add(new ReplaceableAttribute { Name = "Data", Replace = true, Value = @event.Data });
            list.Add(new ReplaceableAttribute { Name = "EventIdentifier", Replace = true, Value = @event.EventIdentifier.ToString() });
            list.Add(new ReplaceableAttribute { Name = "EventSourceId", Replace = true, Value = @event.EventSourceId.ToString() });
            list.Add(new ReplaceableAttribute { Name = "Sequence", Replace = true, Value = @event.Sequence.ToString(CultureInfo.InvariantCulture) });
            list.Add(new ReplaceableAttribute { Name = "Timestamp", Replace = true, Value = @event.Timestamp.ToString(CultureInfo.InvariantCulture) });
            list.Add(new ReplaceableAttribute { Name = "Version", Replace = true, Value = @event.Version });

            _account.PutAttributes(
                new PutAttributesRequest
                    {
                        Attribute = list,
                        DomainName = EVENTTABLENAME,
                        ItemName = @event.EventIdentifier.ToString()
                    });
        }

        public void SaveSource(NcqrsEventSource source)
        {
            if (_commitId == Guid.Empty)
            {
                throw new InvalidOperationException("Cannot Add event sources without beginning a commit.  Call the BeginCommit method");
            }

            List<ReplaceableAttribute> list =
                new List<ReplaceableAttribute>();
            list.Add(new ReplaceableAttribute { Name = "EventSourceId", Replace = true, Value = source.EventSourceId.ToString() });
            list.Add(new ReplaceableAttribute { Name = "Name", Replace = true, Value = source.Name });
            list.Add(new ReplaceableAttribute { Name = "Timestamp", Replace = true, Value = source.Timestamp.ToString(CultureInfo.InvariantCulture) });
            list.Add(new ReplaceableAttribute { Name = "Version", Replace = true, Value = source.Version.ToString(CultureInfo.InvariantCulture) });

            _account.PutAttributes(
                new PutAttributesRequest
                {
                    Attribute = list,
                    DomainName = EVENTSOURCETABLENAME,
                    ItemName = source.EventSourceId.ToString()
                });
        }

        public void EndCommit()
        {
            // No trx - can't commit
        }
    }
}

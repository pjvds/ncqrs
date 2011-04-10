using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain.Storage;
using Ncqrs.Domain;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Microsoft.ApplicationServer.Caching;
using System.Threading;

namespace Ncqrs.Extensions.Azure.Domain
{
    public class CachedUnitOfWork : UnitOfWork
    {
        private string GetCacheKey(Guid eventSourceId)
        {
            return eventSourceId.ToString();
        }

        private string GetCacheKey(AggregateRoot item)
        {
            return GetCacheKey(item.EventSourceId);
        }

        private IDictionary<AggregateRoot, DataCacheLockHandle> _lockedItems = new Dictionary<AggregateRoot, DataCacheLockHandle>();

        public override AggregateRoot GetById(Type aggregateRootType, Guid eventSourceId, long? lastKnownRevision)
        {
            AggregateRoot item = _lockedItems.Keys.Where(k => k.EventSourceId == eventSourceId).FirstOrDefault();

            if (item != null)
            {
                return item;
            }
            DataCacheLockHandle itemLock = null;
            
            try
            {
                // 628426 10 April 2011 avoid exceptions where possible, even though double-roundtrip to cache
                // is undoubtably more expensive than swallowing this exception.
                // TODO: AppFabric Team provide an overload where GetAndLock fails silently if the key doesn't
                // exist
                if (_cache[GetCacheKey(eventSourceId)] != null) 
                {
                    item = (AggregateRoot)_cache.GetAndLock(GetCacheKey(eventSourceId),
                        _timeout,
                        out itemLock);
                }
            }
            catch (DataCacheException)
            {
                // assume it means key didn't exist...
            }

            if (item != null)
            {
                _lockedItems.Add(item, itemLock);
                return item;
            }
            else
            {
                item = base.GetById(aggregateRootType, eventSourceId, lastKnownRevision);
                _cache.Put(GetCacheKey(item.EventSourceId), item );
                return this.GetById(aggregateRootType, eventSourceId, lastKnownRevision);
            }
        }

        protected override void AggregateRootEventAppliedHandler(AggregateRoot aggregateRoot, Eventing.UncommittedEvent evnt)
        {
            base.AggregateRootEventAppliedHandler(aggregateRoot, evnt);

        }

        public override void Accept()
        {
            base.Accept();

            foreach (KeyValuePair<AggregateRoot, DataCacheLockHandle> lockedItem in _lockedItems)
            {
                lockedItem.Key.AcceptChanges();
                _cache.PutAndUnlock(GetCacheKey(lockedItem.Key),
                    lockedItem.Key,
                    lockedItem.Value);
            }
        }



        private DataCache _cache = null;
        private TimeSpan _timeout = TimeSpan.MinValue;
        public CachedUnitOfWork(Guid commandId,
            IDomainRepository domainRepository,
            IEventStore eventStore,
            ISnapshotStore snapshotStore,
            IEventBus eventBus,
            ISnapshottingPolicy snapshottingPolicy,
            DataCache cache,
            TimeSpan timeout)
            : base(commandId,
                domainRepository,
                eventStore,
                snapshotStore,
                eventBus,
                snapshottingPolicy)
        {
            _cache = cache;
            _timeout = timeout;
        }

    }
}

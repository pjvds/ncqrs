using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Ncqrs.Domain.Storage;
using Microsoft.ApplicationServer.Caching;

namespace Ncqrs.Extensions.Azure.Domain
{
    public class CachedUnitOfWorkFactory : IUnitOfWorkFactory
    {
        public CachedUnitOfWorkFactory() : this(null, 1, new TimeSpan(0,0,30))
        {
        }

        public CachedUnitOfWorkFactory(TimeSpan timeout)
            : this(null, 1, timeout)
        {
        }


        public CachedUnitOfWorkFactory(DataCacheFactoryConfiguration config) : this(config, 1, new TimeSpan(0,0,30))
        {
        }
        public CachedUnitOfWorkFactory(DataCacheFactoryConfiguration config, TimeSpan timeout)
            : this(config, 1, timeout)
        {
        }


        private int _numCaches = 0;
        private TimeSpan _timeout = TimeSpan.MinValue;
        public CachedUnitOfWorkFactory(DataCacheFactoryConfiguration config, int numCaches, TimeSpan timeout)
        {
            _cache = new DataCache[numCaches];
            _timeout = timeout;
            _numCaches = numCaches;
            if (config != null)
            {
                _factory = new DataCacheFactory(config);
            }
            else
            {
                // From the dataclientcache .config entry
                _factory = new DataCacheFactory();
            }

            for (int i = 0; i < _numCaches; i++)
            {
                _cache[i] = _factory.GetDefaultCache();
            }

        }

        private DataCache[] _cache = null;
        private DataCacheFactory _factory = null;
        private int _lastCache = 0;
        public DataCache Cache
        {
            get
            {
                // 628426 10 Apr 2011 No need to synchronise here, it doesn't matter if there are gaps
                // in the round-robin cache selection
                unchecked
                {
                    _lastCache++;
                }
                return _cache[_lastCache % _numCaches];
            }
        }
       
        
        public IUnitOfWorkContext CreateUnitOfWork(Guid commandId)
        {
            if (UnitOfWorkContext.Current != null)
            {
                throw new InvalidOperationException("There is already a unit of work created for this context.");
            }

            var store = NcqrsEnvironment.Get<IEventStore>();
            var bus = NcqrsEnvironment.Get<IEventBus>();
            var snapshotStore = NcqrsEnvironment.Get<ISnapshotStore>();
            var snapshottingPolicy = NcqrsEnvironment.Get<ISnapshottingPolicy>();
            var aggregateCreationStrategy = NcqrsEnvironment.Get<IAggregateRootCreationStrategy>();
            var aggregateSnappshotter = NcqrsEnvironment.Get<IAggregateSnapshotter>();

            var repository = new DomainRepository(aggregateCreationStrategy, aggregateSnappshotter);
            var unitOfWork = new CachedUnitOfWork(commandId, repository, store, snapshotStore, bus, snapshottingPolicy, Cache, _timeout);
            UnitOfWorkContext.Bind(unitOfWork);
            return unitOfWork;
        }
    }
}

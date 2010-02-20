using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain.Storage;

namespace Ncqrs.Domain
{
    public class UnitOfWork : IDisposable
    {
        [ThreadStatic]
        private static UnitOfWork _threadInstance;

        private readonly Queue<AggregateRoot> _dirtyInstances;
        private readonly IDomainRepository _repository;

        internal static UnitOfWork Current
        {
            get
            {
                return _threadInstance;
            }
        }

        public IDomainRepository DomainRepository
        {
            get
            {
                return _repository;
            }
        }

        public UnitOfWork(IDomainRepository domainRepository)
        {
            if (Current != null)
                throw new InvalidOperationException("An other UnitOfWork instance already exists in this context.");

            if (domainRepository == null) throw new ArgumentNullException("domainRepository");

            _repository = domainRepository;
            _dirtyInstances = new Queue<AggregateRoot>();
            _threadInstance = this;
        }

        internal void RegisterDirty(AggregateRoot dirtyInstance)
        {
            if (!_dirtyInstances.Contains(dirtyInstance))
            {
                _dirtyInstances.Enqueue(dirtyInstance);
            }
        }

        public void Accept()
        {
            while (_dirtyInstances.Count > 0)
            {
                var dirtyInstance = _dirtyInstances.Dequeue();
                _repository.Save(dirtyInstance);
            }
        }

        public void Dispose()
        {
            _threadInstance = null;
        }
    }
}
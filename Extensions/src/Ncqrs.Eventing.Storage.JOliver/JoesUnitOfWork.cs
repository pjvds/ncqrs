using System;
using Ncqrs.Domain;

namespace Ncqrs.Eventing.Storage.JOliver
{
    public class JoesUnitOfWork : IUnitOfWorkContext
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public AggregateRoot GetById(Type aggregateRootType, Guid eventSourceId, long? lastKnownRevision)
        {
            throw new NotImplementedException();
        }

        public void Accept()
        {
            throw new NotImplementedException();
        }
    }
}
using System;

namespace Ncqrs.Domain
{
    public interface IAggregateRootMixin
    {
        void Initialize(Type aggregateRootPocoType, object aggregateRootPocoInstance);
    }
}
using System;

namespace Ncqrs.Domain
{
    public interface IAggregateRoot
    {
    }

    public static class AggregateRootExtensions
    {
        public static void ApplyEvent(this IAggregateRoot aggregateRoot, DomainEvent evnt)
        {
            ((IAggregateRootInternal)aggregateRoot).ApplyEvent(evnt);
        }

        public static void SetId(this IAggregateRoot aggregateRoot, Guid id)
        {
            ((IAggregateRootInternal) aggregateRoot).Id = id;
        }
    }
}
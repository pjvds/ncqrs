using System;

namespace Ncqrs.Domain
{
    public interface IAggregateRootInternal
    {
        Guid Id { get; set; }
        void ApplyEvent(DomainEvent evnt);
        void RegisterHandler(IDomainEventHandler handler);
    }
}
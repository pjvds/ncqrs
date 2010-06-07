namespace Ncqrs.Domain
{
    public interface IAggregateRootInternal
    {
        void ApplyEvent(DomainEvent evnt);
    }
}
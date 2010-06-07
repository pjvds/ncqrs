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
    }
}
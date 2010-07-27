using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Sourcing.Mapping;

namespace Ncqrs.Domain
{
    /// <summary>
    /// The abstract concept of an aggregate root.
    /// </summary>
    public abstract class AggregateRoot : EventSource
    {
        [NoEventHandler]
        protected override void OnEventApplied(SourcedEvent appliedEvent)
        {
            if (UnitOfWork.Current == null)
                throw new NoUnitOfWorkAvailableInThisContextException();

            UnitOfWork.Current.RegisterDirtyInstance(this);
        }
    }
}
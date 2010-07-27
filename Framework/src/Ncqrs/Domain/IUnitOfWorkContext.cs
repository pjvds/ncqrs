using System;

namespace Ncqrs.Domain
{
    /// <summary>
    /// All changes that will be made within this context are tracked and can be accepted or discarded.
    /// To accept the changes call the <see cref="Accept"/> method before <see cref="IDisposable.Dispose"/>.
    /// To discard the changes simply call <see cref="IDisposable.Dispose"/>.
    /// </summary>
    public interface IUnitOfWorkContext : IDisposable
    {
        /// <summary>
        /// Gets aggregate root by eventSourceId.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
        /// <param name="eventSourceId">The eventSourceId of the aggregate root.</param>
        /// <exception cref="AggregateRootNotFoundException">Occurs when the aggregate root with the 
        /// specified event source id could not be found.</exception>
        /// <returns>
        /// A new instance of the aggregate root that contains the latest known state.
        /// </returns>
        TAggregateRoot GetById<TAggregateRoot>(Guid eventSourceId) where TAggregateRoot : AggregateRoot;

        /// <summary>
        /// Gets aggregate root by <see cref="AggregateRoot.EventSourcId">event source id</see>.
        /// </summary>
        /// <param name="aggregateRootType">Type of the aggregate root.</param>
        /// <param name="eventSourceId">The eventSourceId of the aggregate root.</param>
        /// <exception cref="AggregateRootNotFoundException">Occurs when the aggregate root with the 
        /// specified event source id could not be found.</exception>
        /// <returns>
        /// A new instance of the aggregate root that contains the latest known state.
        /// </returns>
        AggregateRoot GetById(Type aggregateRootType, Guid eventSourceId);

        /// <summary>
        /// Accept all the changes that has been made within this context. All <see cref="IEvent">events</see>
        /// that has been occured will be stored and published.
        /// </summary>
        void Accept();
    }
}
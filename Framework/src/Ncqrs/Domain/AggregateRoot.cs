using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Ncqrs.Eventing.ServiceModel.Bus.Mapping;

namespace Ncqrs.Eventing.ServiceModel.Bus
{
    /// <summary>
    /// The abstract concept of an aggregate root.
    /// </summary>
    public abstract class AggregateRoot : EventSource
    {
        /// <summary>
        /// A list that contains all the event handlers.
        /// </summary>
        private readonly List<IInternalEventHandler> _eventHandlers = new List<IInternalEventHandler>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
        /// </summary>
        /// <remarks>
        /// This instance will be initialized with the <see cref="BasicGuidGenerator"/>.
        /// </remarks>
        protected AggregateRoot() : this(new BasicGuidGenerator())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
        /// </summary>
        /// <param name="idGenerator">The id generator that will be used to generate a new id for this instance.</param>
        protected AggregateRoot(IUniqueIdentifierGenerator idGenerator) : base(idGenerator)
        {
        }

        protected AggregateRoot(IEnumerable<HistoricalEvent> history) : base(history)
        {
        }

        protected void RegisterHandler(IInternalEventHandler handler)
        {
            Contract.Requires<ArgumentNullException>(handler != null, "The handler cannot be null.");

            _eventHandlers.Add(handler);
        }

        protected override void HandleEvent(IEvent evnt)
        {
            Contract.Requires<ArgumentNullException>(evnt != null, "The evnt cannot be null.");
            Boolean handled = false;

            foreach(var handler in _eventHandlers)
            {
                handled |= handler.HandleEvent(evnt);
            }

            if (!handled)
                throw new EventNotHandledException(evnt);
        }

        [NoEventHandler]
        protected override void OnEventApplied(IEvent evnt)
        {
            // Register this instance as a dirty one.
            UnitOfWork.Current.RegisterDirtyInstance(this);

            // Call base.
            base.OnEventApplied(evnt);
        }
    }
}
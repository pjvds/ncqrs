using System;
using Ncqrs.Domain.Mapping;
using System.Collections.Generic;
using Ncqrs.Domain;
using Ncqrs.Domain.Storage;
using System.Diagnostics.Contracts;

namespace Ncqrs.Domain
{
    /// <summary>
    /// The abstract concept of an aggregate root.
    /// </summary>
    public abstract class AggregateRoot : EventSource
    {
        /// <summary>
        /// A dictionary that contains the handlers for events.
        /// </summary>
        private readonly Dictionary<Type, Action<IEvent>> _handlers = new Dictionary<Type, Action<IEvent>>(0);

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

        protected void RegisterHandler<T>(Action<T> handler) where T : IEvent
        {
            if (handler == null) throw new ArgumentNullException("handler");
            var eventType = typeof(T);

            RegisterHandler(eventType, (evnt) => handler((T)evnt));
        }

        protected void RegisterHandler(Type eventType, Action<IEvent> handler)
        {
            Contract.Requires<ArgumentNullException>(eventType != null, "The eventType cannot be null.");
            Contract.Requires<ArgumentNullException>(handler != null, "The handler cannot be null.");
            Contract.Requires<ArgumentException>(typeof(IEvent).IsAssignableFrom(eventType), "The eventType should implement the IEvent interface.");

            if (_handlers.ContainsKey(eventType)) throw new EventHandlerAlreadyRegisterException("");// TODO: More details.

            _handlers.Add(eventType, handler);
        }

        protected override void HandleEvent(IEvent evnt)
        {
            Contract.Requires<ArgumentNullException>(evnt != null, "The evnt cannot be null.");

            var handler = GetHandlerForEvent(evnt);
            handler(evnt);
        }

        private Action<IEvent> GetHandlerForEvent(IEvent evnt)
        {
            Action<IEvent> handler;
            Type eventType = evnt.GetType();

            if (!_handlers.TryGetValue(eventType, out handler))
            {
                throw new NoEventHandlerFoundException(evnt);
            }

            return handler;
        }

        protected override void OnEventApplied(IEvent evnt)
        {
            // Register this instance as a dirty one.
            UnitOfWork.Current.RegisterDirtyInstance(this);

            // Call base.
            base.OnEventApplied(evnt);
        }
    }
}
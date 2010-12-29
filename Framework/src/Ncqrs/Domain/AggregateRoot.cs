using System;
using System.Collections.Generic;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Sourcing.Mapping;

namespace Ncqrs.Domain
{
    /// <summary>
    /// The abstract concept of an aggregate root.
    /// </summary>
    public abstract class AggregateRoot : EventSource
    {
        [ThreadStatic]
        private static readonly List<Action<AggregateRoot, ISourcedEvent>> _eventAppliedCallbacks = new List<Action<AggregateRoot, ISourcedEvent>>();

        public static void RegisterThreadStaticEventAppliedCallback(Action<AggregateRoot, ISourcedEvent> callback)
        {
            _eventAppliedCallbacks.Add(callback);
        }

        public static void UnregisterThreadStaticEventAppliedCallback(Action<AggregateRoot, ISourcedEvent> callback)
        {
            _eventAppliedCallbacks.Remove(callback);
        }

        protected AggregateRoot()
        {}

        protected AggregateRoot(Guid id) : base(id)
        {}

        [NoEventHandler]
        protected override void OnEventApplied(ISourcedEvent appliedEvent)
        {
            var callbacks = _eventAppliedCallbacks;

            foreach(var callback in callbacks)
            {
                callback(this, appliedEvent);
            }
        }
    }
}
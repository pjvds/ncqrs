using System;
using System.Collections.Generic;
using Ncqrs.Eventing;
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
        private static readonly List<Action<AggregateRoot, UncommittedEvent>> _eventAppliedCallbacks = new List<Action<AggregateRoot, UncommittedEvent>>();

        public static void RegisterThreadStaticEventAppliedCallback(Action<AggregateRoot, UncommittedEvent> callback)
        {
            _eventAppliedCallbacks.Add(callback);
        }

        public static void UnregisterThreadStaticEventAppliedCallback(Action<AggregateRoot, UncommittedEvent> callback)
        {
            _eventAppliedCallbacks.Remove(callback);
        }

        protected AggregateRoot()
        {}

        protected AggregateRoot(Guid id) : base(id)
        {}

        [NoEventHandler]
        protected override void OnEventApplied(UncommittedEvent appliedEvent)
        {
            base.OnEventApplied(appliedEvent);
            var callbacks = _eventAppliedCallbacks;

            foreach(var callback in callbacks)
            {
                callback(this, appliedEvent);
            }
        }
    }
}
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
        private static List<Action<AggregateRoot, ISourcedEvent>> _eventAppliedCallbacks;

        private static List<Action<AggregateRoot, ISourcedEvent>> EventAppliedCallbacks
        {
            get
            {
                if (_eventAppliedCallbacks == null)
                {
                    _eventAppliedCallbacks = new List<Action<AggregateRoot, ISourcedEvent>>();
                }

                return _eventAppliedCallbacks;
            }
        }

        public static void RegisterThreadStaticEventAppliedCallback(Action<AggregateRoot, ISourcedEvent> callback)
        {
            EventAppliedCallbacks.Add(callback);
        }

        public static void UnregisterThreadStaticEventAppliedCallback(Action<AggregateRoot, ISourcedEvent> callback)
        {
            EventAppliedCallbacks.Remove(callback);
        }

        protected AggregateRoot()
        {}

        protected AggregateRoot(Guid id) : base(id)
        {}

        [NoEventHandler]
        protected override void OnEventApplied(ISourcedEvent appliedEvent)
        {
            var callbacks = EventAppliedCallbacks;

            foreach(var callback in callbacks)
            {
                callback(this, appliedEvent);
            }
        }
    }
}
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
        private static List<WeakReference> _eventAppliedHandlers;

        /// <summary>
        /// Occurs when an event was applied to an <see cref="AggregateRoot"/>.
        /// </summary>
        internal static event EventHandler<EventAppliedArgs> EventApplied
        {
            add
            {
                if(_eventAppliedHandlers == null) _eventAppliedHandlers = new List<WeakReference>();

                _eventAppliedHandlers.Add(new WeakReference(value));
            }
            remove
            {
                if (_eventAppliedHandlers == null) _eventAppliedHandlers = new List<WeakReference>();

                _eventAppliedHandlers.RemoveAll(r => r.Target == value);
            }
        }

        protected AggregateRoot()
        {}

        protected AggregateRoot(Guid id) : base(id)
        {}

        [NoEventHandler]
        protected override void OnEventApplied(ISourcedEvent appliedEvent)
        {
            if (_eventAppliedHandlers == null)
            {
                return;
            }

            foreach (var handlerRef in _eventAppliedHandlers)
            {
                if (handlerRef.IsAlive)
                {
                    var handler = (EventHandler<EventAppliedArgs>) handlerRef.Target;
                    handler(this, new EventAppliedArgs(appliedEvent));
                }
            }
        }
    }
}
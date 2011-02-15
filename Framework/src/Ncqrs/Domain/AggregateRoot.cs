using System;
using System.Collections.Generic;
using System.Reflection;
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
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [ThreadStatic]
        private static List<Action<AggregateRoot, UncommittedEvent>> _eventAppliedCallbacks;
        private static List<Action<AggregateRoot, UncommittedEvent>> EventAppliedCallbacks
        {
            get
            {
                if (_eventAppliedCallbacks == null)
                {
                    _eventAppliedCallbacks = new List<Action<AggregateRoot, UncommittedEvent>>();
                }
                return _eventAppliedCallbacks;
            }
        }

        public static void RegisterThreadStaticEventAppliedCallback(Action<AggregateRoot, UncommittedEvent> callback)
        {
            Log.DebugFormat("Registering event applied callback {0}", callback.GetHashCode());
            EventAppliedCallbacks.Add(callback);
        }

        public static void UnregisterThreadStaticEventAppliedCallback(Action<AggregateRoot, UncommittedEvent> callback)
        {
            Log.DebugFormat("Deregistering event applied callback {0}", callback.GetHashCode());
            EventAppliedCallbacks.Remove(callback);
        }

        protected AggregateRoot()
        {}

        protected AggregateRoot(Guid id) : base(id)
        {}

        [NoEventHandler]
        protected override void OnEventApplied(UncommittedEvent appliedEvent)
        {
            base.OnEventApplied(appliedEvent);
            var callbacks = EventAppliedCallbacks;

            foreach(var callback in callbacks)
            {
                Log.DebugFormat("Calling event applied callback {0} for event {1} in aggregate root {2}", callback.GetHashCode(), appliedEvent, this);
                callback(this, appliedEvent);
            }
        }
    }
}
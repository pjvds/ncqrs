using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Ncqrs.Domain;

namespace Ncqrs.Eventing.Sourcing
{
    public abstract class EventSource : IEventSource
    {
        [NonSerialized]
        private Guid _eventSourceId;

        /// <summary>
        /// Holds the events that are not yet committed.
        /// </summary>
        [NonSerialized]
        private readonly SourcedEventStream _uncommittedEvents = new SourcedEventStream();

        /// <summary>
        /// Gets the globally unique identifier.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when setting this
        /// value when the version of this aggregate root is not 0 or this
        /// instance contains are any uncommitted events.</exception>
        public Guid EventSourceId
        {
            get { return _eventSourceId; }
            protected set
            {
                Contract.Requires<InvalidOperationException>(Version == 0);

                _eventSourceId = value;
                _uncommittedEvents.EventSourceId = EventSourceId;
            }
        }

        /// <summary>
        /// Gets the current version of the instance as it is known in the event store.
        /// </summary>
        /// <value>
        /// An <see cref="long"/> representing the current version of this aggregate root.
        /// </value>
        public long Version
        {
            get
            {
                return InitialVersion + _uncommittedEvents.Count;
            }
        }
        [NonSerialized]
        private long _initialVersion;

        /// <summary>
        /// Gets the initial version.
        /// <para>
        /// This represents the current version of this instance. When this instance was retrieved
        /// via history, it contains the version as it was at that time. For new instances this value is always 0.
        /// </para>
        /// 	<para>
        /// The version does not change until changes are accepted via the <see cref="AcceptChanges"/> method.
        /// </para>
        /// </summary>
        /// <value>The initial version.</value>
        public long InitialVersion
        {
            get { return _initialVersion; }
            protected set
            {
                Contract.Requires<InvalidOperationException>(Version == InitialVersion);
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                _initialVersion = value;
                _uncommittedEvents.SequenceOffset = value;
            }
        }

        /// <summary>
        /// A list that contains all the event handlers.
        /// </summary>
        [NonSerialized]
        private readonly List<ISourcedEventHandler> _eventHandlers = new List<ISourcedEventHandler>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSource"/> class.
        /// </summary>
        protected EventSource()
        {
            InitialVersion = 0;

            var idGenerator = NcqrsEnvironment.Get<IUniqueIdentifierGenerator>();
            EventSourceId = idGenerator.GenerateNewId();
        }

        [ContractInvariantMethod]
        private void ContractInvariants()
        {
            Contract.Invariant(_uncommittedEvents != null, "The member _unacceptedEvents should never be null.");
        }

        /// <summary>
        /// Initializes from history.
        /// </summary>
        /// <param name="history">The history.</param>
        public virtual void InitializeFromHistory(IEnumerable<SourcedEvent> history)
        {
            Contract.Requires<ArgumentNullException>(history != null, "The history cannot be null.");
            if (_uncommittedEvents.Count > 0) throw new InvalidOperationException("Cannot apply history when instance has uncommitted changes.");

            foreach (var historicalEvent in history)
            {
                if (InitialVersion == 0)
                {
                    EventSourceId = historicalEvent.EventSourceId;
                }

                ApplyEventFromHistory(historicalEvent);
                InitialVersion++; // TODO: Thought... couldn't we get this from the event?
            }
        }

        protected void RegisterHandler(ISourcedEventHandler handler)
        {
            Contract.Requires<ArgumentNullException>(handler != null, "The handler cannot be null.");

            _eventHandlers.Add(handler);
        }

        protected virtual void HandleEvent(SourcedEvent evnt)
        {
            Contract.Requires<ArgumentNullException>(evnt != null, "The Event cannot be null.");
            Boolean handled = false;

            foreach (var handler in _eventHandlers)
            {
                handled |= handler.HandleEvent(evnt);
            }

            if (!handled)
                throw new EventNotHandledException(evnt);
        }

        protected void ApplyEvent(SourcedEvent evnt)
        {
            if (evnt.EventSourceId != SourcedEvent.UndefinedEventSourceId)
            {
                var message = String.Format("The {0} event cannot be applied to event source {1} with id {2} " +
                                            "since it was already owned by event source with id {3}.",
                                            evnt.GetType().FullName, this.GetType().FullName, EventSourceId, evnt.EventSourceId);
                throw new InvalidOperationException(message);
            }

            if (evnt.EventSequence != SourcedEvent.UndefinedEventSequence)
            {
                // TODO: Add better exception message.
                var message = String.Format("The {0} event cannot be applied to event source {1} with id {2} " +
                            "since the event already contains a sequence {3} while {4} was expected.",
                            evnt.GetType().FullName, this.GetType().FullName, EventSourceId, evnt.EventSequence, SourcedEvent.UndefinedEventSequence);
                throw new InvalidOperationException(message);
            }

            evnt.EventSourceId = EventSourceId;
            evnt.EventSequence = Version + 1;

            // First handle event. This to support the set of the
            // Id property for the first event. If we first Append
            // the event to the event stream, the handler cannot set
            // the Id property anymore.
            HandleEvent(evnt);

            _uncommittedEvents.Append(evnt);
            
            OnEventApplied(evnt);
        }

        private void ApplyEventFromHistory(SourcedEvent evnt)
        {
            if (evnt.EventSourceId != EventSourceId)
            {
                var message = String.Format("Cannot apply historical event from other event source.");
                throw new InvalidOperationException(message);
            }

            if (evnt.EventSequence != InitialVersion + 1)
            {
                var message = String.Format("Cannot apply event with sequence {0}. Since the initial version of the " +
                                            "aggregate root is {1}. Only an event with sequence number {2} can be applied.",
                                            evnt.EventSequence, InitialVersion, InitialVersion + 1);
                throw new InvalidOperationException(message);
            }

            HandleEvent(evnt);
        }

        public IEnumerable<SourcedEvent> GetUncommittedEvents()
        {
            Contract.Ensures(Contract.Result<IEnumerable<SourcedEvent>>() != null, "The result of this method should never be null.");

            return _uncommittedEvents;
        }

        public void AcceptChanges()
        {
            // Get current version, since when we clear 
            // the uncommited event stack the version will 
            // be InitialVersion+0. Since the version is 
            // the result of InitialVersion+number of uncommited events.
            long newInitialVersion = Version;

            _uncommittedEvents.Clear();

            InitialVersion = newInitialVersion;
        }

        protected virtual void OnEventApplied(SourcedEvent appliedEvent)
        {
            // Nothing to do. Allow override from subclassers.
        }
    }
}

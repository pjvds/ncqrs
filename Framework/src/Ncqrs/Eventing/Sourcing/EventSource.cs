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
                return _currentVersion;
            }
        }
        [NonSerialized]
        private long _initialVersion;

        [NonSerialized]
        private long _currentVersion;

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
        }

        /// <summary>
        /// A list that contains all the event handlers.
        /// </summary>
        [NonSerialized]
        private readonly List<ISourcedEventHandler> _eventHandlers = new List<ISourcedEventHandler>();
        [NonSerialized]
        private readonly IUniqueIdentifierGenerator _idGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSource"/> class.
        /// </summary>
        protected EventSource()
        {
            _idGenerator = NcqrsEnvironment.Get<IUniqueIdentifierGenerator>();
            EventSourceId = _idGenerator.GenerateNewId();
        }

        protected EventSource(Guid eventSourceId) 
            : this()
        {
            EventSourceId = eventSourceId;
        }

        /// <summary>
        /// Initializes from history.
        /// </summary>
        /// <param name="history">The history.</param>
        public virtual void InitializeFromHistory(CommittedEventStream history)
        {
            Contract.Requires<ArgumentNullException>(history != null, "The history cannot be null.");
            if (_initialVersion != 0) throw new InvalidOperationException("Cannot apply history when instance has uncommitted changes.");

            if (history.IsEmpy)
            {
                return;                
            }

            _eventSourceId = history.SourceId;

            foreach (var historicalEvent in history)
            {                
                ApplyEventFromHistory(historicalEvent);                
            }

            _initialVersion = history.CurrentSourceVersion;
        }

        public event EventHandler<EventAppliedEventArgs> EventApplied;

        protected virtual void OnEventApplied(UncommittedEvent evnt)
        {
            if (EventApplied != null)
            {
                EventApplied(this, new EventAppliedEventArgs(evnt));
            }
        }

        internal protected void RegisterHandler(ISourcedEventHandler handler)
        {
            Contract.Requires<ArgumentNullException>(handler != null, "The handler cannot be null.");

            _eventHandlers.Add(handler);
        }

        protected virtual void HandleEvent(object evnt)
        {
            Contract.Requires<ArgumentNullException>(evnt != null, "The Event cannot be null.");
            Boolean handled = false;

            // Get a copy of the handlers because an event
            // handler can register a new handler. This will
            // cause the _eventHandlers list to be modified.
            // And modification while iterating it not allowed.
            var handlers = new List<ISourcedEventHandler>(_eventHandlers);

            foreach (var handler in handlers)
            {
                handled |= handler.HandleEvent(evnt);
            }

            if (!handled)
                throw new EventNotHandledException(evnt);
        }

        internal protected void ApplyEvent(object evnt)
        {
            var wrappedEvent = new UncommittedEvent(_idGenerator.GenerateNewId(), EventSourceId, GetNextSequence(), _initialVersion, DateTime.UtcNow, evnt);

            HandleEvent(wrappedEvent.Payload);
            OnEventApplied(wrappedEvent);
        }

        private long GetNextSequence()
        {
            _currentVersion++;
            return _currentVersion;
        }

        private void ApplyEventFromHistory(CommittedEvent evnt)
        {
            ValidateHistoricalEvent(evnt);
            HandleEvent(evnt.Payload);
        }

        private void ValidateHistoricalEvent(CommittedEvent evnt)
        {
            if (evnt.EventSourceId != EventSourceId)
            {
                var message = String.Format("Cannot apply historical event from other event source.");
                throw new InvalidOperationException(message);
            }

            //Do we really really need this check? Why don't we trust IEventStore?

            //if (evnt.EventSequence != InitialVersion + 1)
            //{
            //    var message = String.Format("Cannot apply event with sequence {0}. Since the initial version of the " +
            //                                "aggregate root is {1}. Only an event with sequence number {2} can be applied.",
            //                                evnt.EventSequence, InitialVersion, InitialVersion + 1);
            //    throw new InvalidOperationException(message);
            //}
        }
        
        public void AcceptChanges()
        {            
            _initialVersion = Version;
        }        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

namespace Ncqrs.Eventing
{
    public abstract class EventSource
    {
        public event EventAppliedEventHandler EventApplied;
        public event HistoricalEventAppliedEventHandler HistoricalEventApplied;
        public event EventsAcceptedEventHandler EventsAccepted;

        /// <summary>
        /// Gets the globally unique identifier.
        /// </summary>
        public Guid Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is initializing from history.
        /// </summary>
        /// <remarks>This will be set to true at the beginning of the <see cref="InitializeFromHistory"/> method and set to false at the end.</remarks>
        /// <value>
        /// 	<c>true</c> if this instance is initializing from history; otherwise, <c>false</c>.
        /// </value>
        protected Boolean InitializingFromHistory
        {
            get;
            private set;
        }

        /// <summary>
        /// Holds the events that are not yet accepted.
        /// </summary>
        private readonly Stack<IEvent> _unacceptedEvents = new Stack<IEvent>(0);

        /// <summary>
        /// Gets the current version.
        /// </summary>
        /// <value>An <see cref="int"/> representing the current version of the <see cref="EventSource"/>.</value>
        public long Version
        {
            get;
            private set;
        }

        protected EventSource(IUniqueIdentifierGenerator idGenerator)
        {
            if (idGenerator == null) throw new ArgumentNullException("idGenerator");

            Id = idGenerator.GenerateNewId(this);
            Version = 0;
        }

        protected EventSource(IEnumerable<HistoricalEvent> history)
        {
            Contract.Requires<ArgumentNullException>(history != null);

            InitializeFromHistory(history);
        }

        /// <summary>
        /// Initializes from history.
        /// </summary>
        /// <param name="history">The history.</param>
        protected virtual void InitializeFromHistory(IEnumerable<HistoricalEvent> history)
        {
            if (history == null) throw new ArgumentNullException("history");
            if (history.Count() == 0) throw new ArgumentException("The provided history does not contain any historical event.", "history");
            if (Version != 0 || _unacceptedEvents.Count > 0) throw new InvalidOperationException("Cannot load from history when a event source is already loaded.");

            try
            {
                InitializingFromHistory = true;

                foreach (var historicalEvent in history)
                {
                    ApplyHistoricalEvent(historicalEvent);
                    Version++;
                }
            }
            finally
            {
                InitializingFromHistory = false;
            }
        }

        protected abstract void HandleEvent(IEvent evnt);

        protected void ApplyHistoricalEvent(HistoricalEvent evnt)
        {
            if (evnt == null) throw new ArgumentNullException("event");
            HandleEvent(evnt.Event);

            OnHistoricalEventApplied(evnt);
        }

        protected void ApplyEvent(IEvent evnt)
        {
            if (evnt == null) throw new ArgumentNullException("event");
            HandleEvent(evnt);

            _unacceptedEvents.Push(evnt);

            OnEventApplied(evnt);
        }

        public IEnumerable<IEvent> GetUncommitedEvents()
        {
            return _unacceptedEvents;
        }

        public void AcceptEvents()
        {
            // Grab events that will be accepted.
            IEnumerable<IEvent> acceptedEvents = GetUncommitedEvents();

            // Clear the unaccepted event list.
            _unacceptedEvents.Clear();

            // Notify the world that the events are accepted.
            OnEventsAccepted(acceptedEvents);
        }

        protected void OverrideId(Guid id)
        {
            Id = id;
        }

        protected virtual void OnEventApplied(IEvent evnt)
        {
            if(EventApplied != null)
            {
                EventApplied(this, new EventAppliedEventArgs(evnt));
            }
        }

        protected virtual void OnHistoricalEventApplied(HistoricalEvent historicalEvent)
        {
            if (HistoricalEventApplied != null)
            {
                HistoricalEventApplied(this, new HistoricalEventAppliedEventArgs(historicalEvent));
            }
        }

        protected virtual void OnEventsAccepted(IEnumerable<IEvent> acceptedEvents)
        {
            if (EventsAccepted != null)
            {
                EventsAccepted(this, new EventsAcceptedEventArgs(acceptedEvents));
            }
        }
    }
}
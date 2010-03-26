using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

namespace Ncqrs.Eventing
{
    public abstract class EventSource
    {
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
        public int Version
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
            Contract.Requires<ArgumentNullException>(history == null);

            InitializeFromHistory(history);
        }

        /// <summary>
        /// Initializes from history.
        /// </summary>
        /// <param name="history">The history.</param>
        protected void InitializeFromHistory(IEnumerable<HistoricalEvent> history)
        {
            if (history == null) throw new ArgumentNullException("history");
            if (history.Count() == 0) throw new ArgumentException("The provided history does not contain any historical event.", "history");
            if (Version != 0 || _unacceptedEvents.Count > 0) throw new InvalidOperationException("Cannot load from history when a event source is already loaded.");

            try
            {
                InitializingFromHistory = true;

                foreach (var historicalEvent in history)
                {
                    ApplyEvent(historicalEvent);
                }
            }
            finally
            {
                InitializingFromHistory = false;
            }
        }

        protected abstract void HandleEvent(IEvent evnt);

        protected void ApplyEvent(HistoricalEvent evnt)
        {
            if (evnt == null) throw new ArgumentNullException("event");
            HandleEvent(evnt.Event);
        }

        protected void ApplyEvent(IEvent evnt)
        {
            if (evnt == null) throw new ArgumentNullException("event");
            HandleEvent(evnt);

            _unacceptedEvents.Push(evnt);
        }

        public IEnumerable<IEvent> GetUncommitedEvents()
        {
            return _unacceptedEvents;
        }

        public void AcceptEvents()
        {
            _unacceptedEvents.Clear();
        }

        protected void OverrideId(Guid id)
        {
            if (!InitializingFromHistory) throw new InvalidOperationException("Instance is not initializing from history.");

            Id = id;
        }
    }
}
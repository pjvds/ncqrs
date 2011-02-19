using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ncqrs.Eventing
{
    /// <summary>
    /// Represents a stream of events which has not been persisted yet. They are to be persisted atomicaly as a single commit with
    /// given <see cref="CommitId"/> ID.
    /// </summary>
    public class UncommittedEventStream : IEnumerable<UncommittedEvent>
    {
        private readonly Guid _commitId;
        private Guid? _singleSource;
        private bool _hasSingleSource = true;
        private readonly List<UncommittedEvent> _events = new List<UncommittedEvent>();
        private readonly Dictionary<Guid, EventSourceInformation> _eventSourceInformation = new Dictionary<Guid, EventSourceInformation>();

        /// <summary>
        /// Creates new uncommitted event stream.
        /// </summary>
        /// <param name="commitId"></param>
        public UncommittedEventStream(Guid commitId)
        {
            _commitId = commitId;
        }

        /// <summary>
        /// Appends new event to the stream.
        /// </summary>
        /// <param name="evnt">New event.</param>
        public void Append(UncommittedEvent evnt)
        {
            if (_events.Count > 0 && _hasSingleSource)
            {
                if (_events[0].EventSourceId != evnt.EventSourceId)
                {
                    _hasSingleSource = false;
                }                
            }
            else if (_events.Count == 0)
            {
                _singleSource = evnt.EventSourceId;
            }
            _events.Add(evnt);
            evnt.OnAppendedToStream(_commitId);
            UpdateEventSourceInformation(evnt);
        }

        private void UpdateEventSourceInformation(UncommittedEvent evnt)
        {
            var newInformation = new EventSourceInformation(evnt.EventSourceId, evnt.InitialVersionOfEventSource, evnt.EventSequence);
            _eventSourceInformation[evnt.EventSourceId] = newInformation;
        }

        /// <summary>
        /// Gets information about sources of events in this stream.
        /// </summary>
        public IEnumerable<EventSourceInformation> Sources
        {
            get { return _eventSourceInformation.Values; }
        }

        /// <summary>
        /// Returns whether this stream of events has a single source. An empty stream has single source by definition.
        /// </summary>
        public bool HasSingleSource
        {
            get
            {
                return _hasSingleSource;
            }
        }

        /// <summary>
        /// If the stream has a single source, it returns this source.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the stream has multiple sources.</exception>
        public Guid SourceId
        {
            get
            {
                if (!HasSingleSource)
                {
                    throw new InvalidOperationException("Event stream must have a single source in order to retrieve its source.");
                }
                return _singleSource.Value;
            }
        }

        public long InitialVersion
        {
            get
            {
                if (!HasSingleSource)
                {
                    throw new InvalidOperationException("Event stream must have a single source in order to retrieve its source initial version.");
                }
                return _eventSourceInformation[SourceId].InitialVersion;
            }
        }

        public long CurrentVersion 
        {
            get
            {
                if (!HasSingleSource)
                {
                    throw new InvalidOperationException("Event stream must have a single source in order to retrieve its source current version.");
                }
                return _eventSourceInformation[SourceId].InitialVersion;
            }
        }

        /// <summary>
        /// Returns if the stream contains at least one event.
        /// </summary>
        public bool IsNotEmpty
        {
            get { return _events.Count > 0; }
        }

        /// <summary>
        /// Returns unique id of commit associated with this stream.
        /// </summary>
        public Guid CommitId
        {
            get { return _commitId; }
        }

        public IEnumerator<UncommittedEvent> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
using System;
using System.Threading.Tasks;

namespace Ncqrs.EventBus
{
    public class EventFetcher
    {
        private readonly IEventFetchPolicy _fetchPolicy;
        private readonly IBrowsableEventStore _eventStore;
        private bool _activeFetchRequest;
        private readonly object _fetchLock = new object();
        private int _eventSequence = 1;

        public EventFetcher(IEventFetchPolicy fetchPolicy, IBrowsableEventStore eventStore)
        {
            _fetchPolicy = fetchPolicy;
            _eventStore = eventStore;
        }

        public void EvaluateEventFetchPolicy(PipelineState pipelineState)
        {
            if (_activeFetchRequest)
            {
                return;
            }
            var directive = _fetchPolicy.ShouldFetch(pipelineState);
            if (directive.ShouldFetch)
            {
                StartFetchingEvents(directive);
            }
        }

        public event EventHandler<EventFetchedEventArgs> EventFetched;

        private void OnEventFetched(EventFetchedEventArgs e)
        {
            EventHandler<EventFetchedEventArgs> handler = EventFetched;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void StartFetchingEvents(FetchDirective directive)
        {
            Task.Factory.StartNew(() => FetchEvents(directive));
        }

        private void FetchEvents(FetchDirective directive)
        {
            lock (_fetchLock)
            {
                _activeFetchRequest = true;
                var events = _eventStore.FetchEvents(directive.MaxCount);
                foreach (var evnt in events)
                {
                    var sequencedEvent = new SequencedEvent(_eventSequence++, evnt);
                    OnEventFetched(new EventFetchedEventArgs(sequencedEvent));
                }
                _activeFetchRequest = false;
            }
        }
    }
}
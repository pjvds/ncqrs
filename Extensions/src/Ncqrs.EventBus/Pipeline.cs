using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Ncqrs.EventBus
{
    public class Pipeline
    {
        private const int MaxDegreeOfParallelismForProcessing = 10;
        private const int PipelineStateUpdateThreshold = 10;
        private readonly EventFetcher _fetcher;
        private readonly PipelineProcessor _processor;
        private readonly IBrowsableEventStore _eventStore;
        private readonly EventDemultiplexer _eventDemultiplexer;
        private readonly BlockingCollection<SequencedEvent> _preProcessingQueue = new BlockingCollection<SequencedEvent>();
        private readonly BlockingCollection<SequencedEvent> _postProcessingQueue = new BlockingCollection<SequencedEvent>();
        private readonly BlockingCollection<Action> _preDemultiplexingQueue = new BlockingCollection<Action>();
        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();

        public Pipeline(IEventProcessor eventProcessor, IBrowsableEventStore eventStore, IEventFetchPolicy fetchPolicy)
        {
            _eventStore = new LazyBrowsableEventStore(eventStore, PipelineStateUpdateThreshold);
            _eventDemultiplexer = new EventDemultiplexer();
            _eventDemultiplexer.EventDemultiplexed += OnEventDemultiplexed;
            _processor = new PipelineProcessor(eventProcessor);
            _processor.EventProcessed += OnEventProcessed;
            _fetcher = new EventFetcher(fetchPolicy, _eventStore);
            _fetcher.EventFetched += OnEventFetched;
        }

        private void OnEventDemultiplexed(object sender, EventDemultiplexedEventArgs e)
        {
            _preProcessingQueue.Add(e.Event);
        }

        private void OnEventProcessed(object sender, EventProcessedEventArgs e)
        {
            _preDemultiplexingQueue.Add(() => _eventDemultiplexer.MarkAsProcessed(e.Event));
            _postProcessingQueue.Add(e.Event);
        }

        private void OnEventFetched(object sender, EventFetchedEventArgs e)
        {
            _preDemultiplexingQueue.Add(() => _eventDemultiplexer.Demultiplex(e.Event));
        }        

        public void Start()
        {
            _fetcher.EvaluateEventFetchPolicy(new PipelineState(0));
            StartProcessor();
            StartDemultiplexer();
            StartPostProcessor();
        }

        public void Stop()
        {
            _cancellation.Cancel();
            _cancellation.Dispose();
        }               

        private void StartDemultiplexer()
        {            
            Task.Factory.StartNew(DemultiplexEventsAndEvauateFetchPolicy, _cancellation.Token, TaskCreationOptions.LongRunning);
        }

        private void DemultiplexEventsAndEvauateFetchPolicy(object cancellationToken)
        {
            var eventStream = _preDemultiplexingQueue.GetConsumingEnumerable((CancellationToken)cancellationToken);
            foreach (var evnt in eventStream)
            {
                evnt();
                _fetcher.EvaluateEventFetchPolicy(new PipelineState(_preDemultiplexingQueue.Count));
            }
        }

        private void StartProcessor()
        {
            for (int i = 0; i < MaxDegreeOfParallelismForProcessing; i++)
            {
                Task.Factory.StartNew(ProcessEvents, _cancellation.Token, TaskCreationOptions.LongRunning);
            }
        }

        private void ProcessEvents(object cancellationToken)
        {
            var eventStream = _preProcessingQueue.GetConsumingEnumerable((CancellationToken)cancellationToken);
            foreach (var evnt in eventStream)
            {
                _processor.ProcessNext(evnt);
            }
        }

        private void StartPostProcessor()
        {
            Task.Factory.StartNew(PostProcessEvents, _cancellation.Token, TaskCreationOptions.LongRunning);
        }                

        private void PostProcessEvents(object cancellationToken)
        {
            var eventStream = _postProcessingQueue.GetConsumingEnumerable((CancellationToken)cancellationToken);
            foreach (var evnt in eventStream)
            {
                _eventStore.MarkLastProcessedEvent(evnt);
            }
        }
    }
}
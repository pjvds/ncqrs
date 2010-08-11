using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ncqrs.EventBus
{
    public class Pipeline
    {
        private const int MaxDegreeOfParallelismForProcessing = 10;
        private const int PipelineStateUpdateThreshold = 10;
        private const int PipelineMonitorThreshold = 1;

        private int _eventSequence;

        private readonly PipelineProcessor _processor;
        private readonly IEventFetchPolicy _fetchPolicy;
        private readonly IEventStore _eventStore;
        private readonly IPipelineStateStore _pipelineStateStore;
        private readonly EventDemultiplexer _eventDemultiplexer;
        private readonly BlockingCollection<SequencedEvent> _preProcessingQueue = new BlockingCollection<SequencedEvent>();
        private readonly BlockingCollection<SequencedEvent> _preDemultiplexingQueue = new BlockingCollection<SequencedEvent>();
        private readonly IPipelineStateMonitor _stateMonitor;
        private readonly IPipelineBackupQueue _pipelineBackupQueue;

        public Pipeline(IEventProcessor eventProcessor, IPipelineBackupQueue pipelineBackupQueue, IPipelineStateStore pipelineStateStore, IEventStore eventStore, IEventFetchPolicy fetchPolicy)
        {
            _stateMonitor = new ThresholedPipelineStateMonitor(PipelineMonitorThreshold);
            _stateMonitor.StateChanged += OnPipelineStateChanged;
            _pipelineStateStore = new ThresholdedPipelineStateStore(pipelineStateStore, PipelineStateUpdateThreshold);
            _eventDemultiplexer = new EventDemultiplexer(EnqueueToProcessing, _stateMonitor);
            _processor = new PipelineProcessor(_pipelineBackupQueue, _pipelineStateStore, eventProcessor, _eventDemultiplexer);
            _pipelineBackupQueue = pipelineBackupQueue;
            _fetchPolicy = fetchPolicy;
            _eventStore = eventStore;
        }

        private void OnPipelineStateChanged(object sender, PipelineStateChangedEventArgs e)
        {
            var directive = _fetchPolicy.ShouldFetch(e.State);
            if (directive.ShouldFetch)
           {
               FetchEvents(directive);
           }
        }

        private void FetchEvents(FetchDirective directive)
        {
            Task.Factory.StartNew(() =>
                                      {
                                          var events = _eventStore.FetchEvents(directive.MaxCount);
                                          foreach (var evnt in events)
                                          {
                                              _preDemultiplexingQueue.Add(new SequencedEvent(_eventSequence++, evnt));
                                          }
                                      });

        }

        public void Start()
        {
            _eventStore.SetCursorPositionAfter(_pipelineStateStore.GetLastProcessedEvent());
            StartProcessor();
            StartDemultiplexer();
        }

        private void EnqueueToProcessing(SequencedEvent evnt)
        {
            _preProcessingQueue.Add(evnt);
        }

        private void StartDemultiplexer()
        {
            var processingTask = Task.Factory.StartNew(DemultiplexEvents);
            processingTask.Wait();
        }

        private void StartProcessor()
        {
            var processingTask = Task.Factory.StartNew(ProcessEvents);
            processingTask.Wait();
        }

        private void DemultiplexEvents()
        {
            var eventStream = _preDemultiplexingQueue.GetConsumingEnumerable();
            foreach (var evnt in eventStream)
            {
                _eventDemultiplexer.ProcessNext(evnt);
            }
        }

        private void ProcessEvents()
        {
            var eventStream = _preProcessingQueue.GetConsumingEnumerable();
            var parallelOptions = new ParallelOptions
                                      {
                                          MaxDegreeOfParallelism = MaxDegreeOfParallelismForProcessing
                                      };
            Parallel.ForEach(eventStream, parallelOptions, x => _processor.ProcessNext(x));
        }
    }
}
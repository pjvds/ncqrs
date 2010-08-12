using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ncqrs.EventBus
{
    public class Pipeline
    {
        private const int MaxDegreeOfParallelismForProcessing = 10;
        private const int PipelineStateUpdateThreshold = 10;
        private static readonly TimeSpan EventFetchPolicyUpdateInterval = TimeSpan.FromMilliseconds(100);

        private int _eventSequence;

        private readonly PipelineProcessor _processor;
        private readonly IEventFetchPolicy _fetchPolicy;
        private readonly IEventStore _eventStore;
        private readonly IPipelineStateStore _pipelineStateStore;
        private readonly EventDemultiplexer _eventDemultiplexer;
        private readonly BlockingCollection<SequencedEvent> _preProcessingQueue = new BlockingCollection<SequencedEvent>();
        private readonly BlockingCollection<SequencedEvent> _postProcessingQueue = new BlockingCollection<SequencedEvent>();
        private readonly BlockingCollection<SequencedEvent> _preDemultiplexingQueue = new BlockingCollection<SequencedEvent>();
        private readonly IPipelineBackupQueue _pipelineBackupQueue;
        private Timer _eventFetchPolicyExecutor;

        public Pipeline(IEventProcessor eventProcessor, IPipelineBackupQueue pipelineBackupQueue, IPipelineStateStore pipelineStateStore, IEventStore eventStore, IEventFetchPolicy fetchPolicy)
        {
            _pipelineStateStore = new ThresholdedPipelineStateStore(pipelineStateStore, PipelineStateUpdateThreshold);
            _eventDemultiplexer = new EventDemultiplexer(EnqueueToProcessing);
            _eventDemultiplexer.StateChanged += (sender, args) => EvaluateEventFetchPolicy();
            _processor = new PipelineProcessor(_pipelineBackupQueue, eventProcessor, _eventDemultiplexer, EnqueueToPostProcessing);
            _pipelineBackupQueue = pipelineBackupQueue;
            _fetchPolicy = fetchPolicy;
            _eventStore = eventStore;
        }

        private void EvaluateEventFetchPolicy()
        {
            var directive = _fetchPolicy.ShouldFetch(new PipelineState(_eventDemultiplexer.PendingEventCount));
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
            StartPostProcessor();
            FetchEvents(FetchDirective.FetchNow(10));
            _eventFetchPolicyExecutor = new Timer(x => EvaluateEventFetchPolicy(), null, TimeSpan.Zero, EventFetchPolicyUpdateInterval);            
        }

        private void EnqueueToProcessing(SequencedEvent evnt)
        {
            _preProcessingQueue.Add(evnt);
        }

        private void EnqueueToPostProcessing(SequencedEvent evnt)
        {
            _postProcessingQueue.Add(evnt);
        }

        private void StartDemultiplexer()
        {
            Task.Factory.StartNew(DemultiplexEvents);
        }

        private void StartProcessor()
        {
            Task.Factory.StartNew(ProcessEvents);
        }

        private void StartPostProcessor()
        {
            Task.Factory.StartNew(PostProcessEvents);
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

        private void PostProcessEvents()
        {
            var eventStream = _postProcessingQueue.GetConsumingEnumerable();
            foreach (var evnt in eventStream)
            {
                _pipelineStateStore.MarkLastProcessedEvent(evnt);
            }
        }
    }
}
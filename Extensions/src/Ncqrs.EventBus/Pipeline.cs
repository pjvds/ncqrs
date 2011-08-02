using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Ncqrs.EventBus
{    
    public class Pipeline
    {
        private const int MaxDegreeOfParallelismForProcessing = 1;
        private readonly ElementFetcher _fetcher;
        private readonly PipelineProcessor _processor;
        private readonly string _name;
        private readonly IBrowsableElementStore _elementStore;
        private readonly Demultiplexer _demultiplexer;
        private readonly BlockingCollection<IProcessingElement> _preProcessingQueue = new BlockingCollection<IProcessingElement>();
        private readonly BlockingCollection<IProcessingElement> _postProcessingQueue = new BlockingCollection<IProcessingElement>();
        private readonly BlockingCollection<Action> _preDemultiplexingQueue = new BlockingCollection<Action>();
        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
        private readonly Timer _fetchTimer;

        public Pipeline(string name, IElementProcessor elementProcessor, IBrowsableElementStore elementStore, IFetchPolicy fetchPolicy)
        {
            _name = name;
            _elementStore = elementStore;
            _demultiplexer = new Demultiplexer();
            _demultiplexer.EventDemultiplexed += OnDemultiplexed;
            _processor = new PipelineProcessor(elementProcessor);
            _processor.EventProcessed += OnEventProcessed;
            _fetcher = new ElementFetcher(fetchPolicy, _elementStore, name);
            _fetcher.ElementFetched += OnElementFetched;
            _fetchTimer = new Timer(x => EvaluateFetchPolicy(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        public static Pipeline CreateWithLazyElementMarking(string name, IElementProcessor elementProcessor, IBrowsableElementStore elementStore)
        {
            return Create(name,elementProcessor, new LazyMarkingBrowsableElementStore(elementStore));
        }

        public static Pipeline Create(string name, IElementProcessor elementProcessor,IBrowsableElementStore elementStore)
        {
            const int minimumPendingEvents = 10;
            const int batchSize = 20;

            return new Pipeline(name, elementProcessor, elementStore, new ThresholdedFetchPolicy(minimumPendingEvents, batchSize));
        }

        private void OnDemultiplexed(object sender, ElementDemultiplexedEventArgs e)
        {
            _preProcessingQueue.Add(e.DemultiplexedElement);
        }

        private void OnEventProcessed(object sender, ElementProcessedEventArgs e)
        {
            _preDemultiplexingQueue.Add(() => _demultiplexer.MarkAsProcessed(e.ProcessedElement));
            _postProcessingQueue.Add(e.ProcessedElement);
        }

        private void OnElementFetched(object sender, ElementFetchedEventArgs e)
        {
            _preDemultiplexingQueue.Add(() => _demultiplexer.Demultiplex(e.ProcessingElement));
        }        

        public void Start()
        {
            StartProcessor();
            StartDemultiplexer();
            StartPostProcessor();
            EvaluateFetchPolicy();
        }

        public void Stop()
        {
            _cancellation.Cancel();
        }               

        private void StartDemultiplexer()
        {            
            Task.Factory.StartNew(DemultiplexEventsAndEvaluateFetchPolicy, _cancellation.Token, TaskCreationOptions.LongRunning);
        }

        private void DemultiplexEventsAndEvaluateFetchPolicy(object cancellationToken)
        {
            try
            {
                var eventStream = _preDemultiplexingQueue.GetConsumingEnumerable((CancellationToken) cancellationToken);
                foreach (var evnt in eventStream)
                {
                    evnt();
                    EvaluateFetchPolicy();
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("DemultiplexEventsAndEvaluateFetchPolicy operation cancelled.");
            }
        }

        private void EvaluateFetchPolicy()
        {
            _fetcher.EvaluateFetchPolicy(new PipelineState(_preDemultiplexingQueue.Count));
        }

        private void StartProcessor()
        {
            for (int i = 0; i < MaxDegreeOfParallelismForProcessing; i++)
            {
                Task.Factory.StartNew(ProcessElements, _cancellation.Token, TaskCreationOptions.LongRunning);
            }
        }

        private void ProcessElements(object cancellationToken)
        {
            try
            {
                var eventStream = _preProcessingQueue.GetConsumingEnumerable((CancellationToken)cancellationToken);
                foreach (var evnt in eventStream)
                {
                    _processor.ProcessNext(evnt);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("ProcessElements operation cancelled.");
            }            
        }

        private void StartPostProcessor()
        {
            Task.Factory.StartNew(PostProcessElements, _cancellation.Token, TaskCreationOptions.LongRunning);
        }                

        private void PostProcessElements(object cancellationToken)
        {
            try
            {
                var eventStream = _postProcessingQueue.GetConsumingEnumerable((CancellationToken)cancellationToken);
                foreach (var evnt in eventStream)
                {
                    _elementStore.MarkLastProcessedElement(_name, evnt);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("PostProcessElements operation cancelled.");
            }
        }
    }
}
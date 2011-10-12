using System;
using System.Threading.Tasks;

namespace Ncqrs.EventBus
{
    public class ElementFetcher
    {
        private readonly IFetchPolicy _fetchPolicy;
        private readonly IBrowsableElementStore _elementStore;
        private bool _activeFetchRequest;
        private readonly object _fetchLock = new object();
        private int _sequence = 1;
        private readonly string _pipelineName;

        public ElementFetcher(IFetchPolicy fetchPolicy, IBrowsableElementStore elementStore, string pipelineName)
        {
            _fetchPolicy = fetchPolicy;
            _pipelineName = pipelineName;
            _elementStore = elementStore;
        }

        public void EvaluateFetchPolicy(PipelineState pipelineState)
        {
            if (_activeFetchRequest)
            {
                return;
            }
            var directive = _fetchPolicy.ShouldFetch(pipelineState);
            if (directive.ShouldFetch)
            {
                StartFetching(directive);
            }
        }

        public event EventHandler<ElementFetchedEventArgs> ElementFetched;

        private void OnElementFetched(ElementFetchedEventArgs e)
        {
            EventHandler<ElementFetchedEventArgs> handler = ElementFetched;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void StartFetching(FetchDirective directive)
        {
            Task.Factory.StartNew(() => Fetch(directive));
        }

        private void Fetch(FetchDirective directive)
        {
            lock (_fetchLock)
            {
                _activeFetchRequest = true;
                try
                {
                    var elements = _elementStore.Fetch(_pipelineName, directive.MaxCount);
                    foreach (var element in elements)
                    {
                        element.SequenceNumber = _sequence++;
                        OnElementFetched(new ElementFetchedEventArgs(element));
                    }
                }
                catch
                {
                }
                finally
                {
                    _activeFetchRequest = false;
                }
            }
        }
    }
}
namespace Ncqrs.EventBus
{
    public struct PipelineState
    {
        private readonly int _pendingEventCount;
        private readonly int _activeFetchRequests;

        public PipelineState(int pendingEventCount, int activeFetchRequests)
        {
            _pendingEventCount = pendingEventCount;
            _activeFetchRequests = activeFetchRequests;
        }

        public int ActiveFetchRequests
        {
            get { return _activeFetchRequests; }
        }

        public int PendingEventCount
        {
            get { return _pendingEventCount; }
        }
    }
}
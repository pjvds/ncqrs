namespace Ncqrs.EventBus
{
    public struct PipelineState
    {
        private readonly int _pendingEventCount;

        public PipelineState(int pendingEventCount)
        {
            _pendingEventCount = pendingEventCount;
        }
        
        public int PendingEventCount
        {
            get { return _pendingEventCount; }
        }
    }
}
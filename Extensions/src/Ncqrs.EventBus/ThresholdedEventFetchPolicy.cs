namespace Ncqrs.EventBus
{
    public class ThresholdedEventFetchPolicy : IEventFetchPolicy
    {
        private readonly int _minimumPendingEvents;
        private readonly int _batchSize;

        public ThresholdedEventFetchPolicy(int minimumPendingEvents, int batchSize)
        {
            _minimumPendingEvents = minimumPendingEvents;
            _batchSize = batchSize;
        }

        public FetchDirective ShouldFetch(PipelineState currentState)
        {
            if (currentState.PendingEventCount < _minimumPendingEvents)
            {
                return FetchDirective.FetchNow(_batchSize);
            }
            return FetchDirective.DoNotFetchYet();
        }
    }
}
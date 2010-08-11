using System;
using System.Threading;

namespace Ncqrs.EventBus
{
    public class ThresholedPipelineStateMonitor : IPipelineStateMonitor
    {
        private readonly int _threshold;
        private int _pendingEventCount;
        private int _lastPendingEventCount;

        public ThresholedPipelineStateMonitor(int threshold)
        {
            _threshold = threshold;
        }

        public void OnDemultiplexed()
        {
            Interlocked.Increment(ref _pendingEventCount);
            OnStateChanged();
        }

        public void OnProcessed()
        {
            Interlocked.Decrement(ref _pendingEventCount);
            OnStateChanged();
        }

        public event EventHandler<PipelineStateChangedEventArgs> StateChanged;

        private void OnStateChanged()
        {
            int pendingEventCount = _pendingEventCount;
            int lastPendingEventCount = _lastPendingEventCount;

            if (pendingEventCount == 0 || Math.Abs(pendingEventCount - lastPendingEventCount) > _threshold)
            {
                _lastPendingEventCount = pendingEventCount;
                if (StateChanged != null)
                {
                    StateChanged(this, new PipelineStateChangedEventArgs(new PipelineState(_pendingEventCount)));
                }
            }
        }
    }
}
using System;

namespace Ncqrs.EventBus
{
    public class PipelineStateMonitor
    {
        private readonly EventDemultiplexer _eventDemultiplexer;
        private int _pendingEventCount;

        public PipelineStateMonitor(EventDemultiplexer eventDemultiplexer)
        {
            _eventDemultiplexer = eventDemultiplexer;
            eventDemultiplexer.StateChanged += OnDemultiplexerStateChanged;
        }

        void OnDemultiplexerStateChanged(object sender, EventArgs e)
        {
            _pendingEventCount = _eventDemultiplexer.PendingEventCount;
        }

        public PipelineState GetCurrentState()
        {
            return new PipelineState(_pendingEventCount);
        }

    }
}
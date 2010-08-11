using System;

namespace Ncqrs.EventBus
{
    public class PipelineStateChangedEventArgs : EventArgs
    {
        private readonly PipelineState _state;

        public PipelineStateChangedEventArgs(PipelineState state)
        {
            _state = state;
        }

        public PipelineState State
        {
            get { return _state; }
        }
    }
}
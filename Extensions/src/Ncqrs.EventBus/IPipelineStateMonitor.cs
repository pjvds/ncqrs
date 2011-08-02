using System;

namespace Ncqrs.EventBus
{
    public interface IPipelineStateMonitor
    {
        void OnDemultiplexed();
        void OnProcessed();
        event EventHandler<PipelineStateChangedEventArgs> StateChanged;
    }
}
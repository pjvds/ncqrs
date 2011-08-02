using System;

namespace Ncqrs.EventBus
{
    public interface IPipelineMonitoringStrategy
    {
        event EventHandler<PipelineStateChangedEventArgs> StateChanged;
    }
}
using System;

namespace Ncqrs.EventBus
{
    public interface IEventFetchPolicy
    {
        FetchDirective ShouldFetch(PipelineState currentState);
    }
}
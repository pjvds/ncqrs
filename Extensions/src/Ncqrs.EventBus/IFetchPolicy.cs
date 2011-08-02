using System;

namespace Ncqrs.EventBus
{
    public interface IFetchPolicy
    {
        FetchDirective ShouldFetch(PipelineState currentState);
        void OnFetchingCompleted(FetchResult result);
    }
}
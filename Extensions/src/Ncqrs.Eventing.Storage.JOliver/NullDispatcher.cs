using EventStore;
using EventStore.Dispatcher;

namespace Ncqrs.Eventing.Storage.JOliver
{
    public class NullDispatcher : IDispatchCommits
    {
        public void Dispose()
        {
        }

        public void Dispatch(Commit commit)
        {
        }
    }
}
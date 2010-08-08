using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus
{
    public interface IEventProcessor
    {
        void Process(SourcedEvent evnt);
    }
}
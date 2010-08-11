namespace Ncqrs.EventBus
{
    public interface IEventQueue
    {
        void MarkAsProcessed(SequencedEvent evnt);
    }
}
namespace Ncqrs.Eventing.ServiceModel.Bus
{
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        void Handle(TEvent evnt);
    }
}

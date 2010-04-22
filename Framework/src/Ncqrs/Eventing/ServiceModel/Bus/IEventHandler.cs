namespace Ncqrs.Eventing.ServiceModel.Bus
{
    public interface IEventHandler
    {
        void Handle(IEvent eventMessage);
    }
}

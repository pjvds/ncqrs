namespace Ncqrs.Eventing.ServiceModel.Bus
{
    public interface IEventBusFactory
    {
        IEventBus CreateEventBus();
    }
}
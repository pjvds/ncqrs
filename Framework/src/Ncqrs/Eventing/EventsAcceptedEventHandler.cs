using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Eventing
{
    public delegate void EventsAcceptedEventHandler(EventSource sender, EventsAcceptedEventArgs e);
}

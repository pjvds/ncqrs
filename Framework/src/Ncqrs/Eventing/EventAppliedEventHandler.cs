using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Eventing
{
    public delegate void EventAppliedEventHandler(EventSource sender, EventAppliedEventArgs e);
}

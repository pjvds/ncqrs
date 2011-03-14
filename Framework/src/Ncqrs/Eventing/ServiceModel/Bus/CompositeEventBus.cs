using System.Collections.Generic;

namespace Ncqrs.Eventing.ServiceModel.Bus
{
    /// <summary>
    /// <see cref="IEventBus"/> implementation wrapping serveral other event buses.
    /// </summary>
    public class CompositeEventBus : IEventBus
    {
        private readonly List<IEventBus> _wrappedBuses = new List<IEventBus>();        

        /// <summary>
        /// Adds a bus to the list of buses.
        /// </summary>
        /// <param name="bus">A bus to be added.</param>
        public void AddBus(IEventBus bus)
        {
            _wrappedBuses.Add(bus);
        }

        /// <summary>
        /// Removes a bus from the list of buses.
        /// </summary>
        /// <param name="bus">A bus to be removed.</param>
        public void RemoveBus(IEventBus bus)
        {
            _wrappedBuses.Remove(bus);
        }

        public void Publish(IPublishableEvent eventMessage)
        {
            foreach (var bus in _wrappedBuses)
            {
                bus.Publish(eventMessage);
            }
        }

        public void Publish(IEnumerable<IPublishableEvent> eventMessages)
        {
            foreach (var bus in _wrappedBuses)
            {
                bus.Publish(eventMessages);
            }
        }
    }
}
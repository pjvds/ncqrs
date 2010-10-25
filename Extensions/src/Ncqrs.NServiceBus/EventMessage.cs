using System;
using Ncqrs.Eventing;
using nsb = NServiceBus;

namespace Ncqrs.NServiceBus
{
    public interface INsbEventMessage<T> : nsb.IMessage
where T : Ncqrs.Eventing.IEvent
    {
        T Payload { get; set; }
    }



    /// <summary>
    /// Wraps Ncqrs event to be transportable by NServiceBus.
    /// </summary>
    /// <typeparam name="T">Type of transported event.</typeparam>
    [Serializable]
    public class EventMessage<T> : Ncqrs.NServiceBus.INsbEventMessage<T>
       where T : IEvent
    {
        /// <summary>
        /// Gets or sets transported event.
        /// </summary>
        public T Payload { get; set; }
    }


}
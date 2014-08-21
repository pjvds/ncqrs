using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;
using NServiceBus;
using IEvent = NServiceBus.IEvent;

//using IEvent = NServiceBus.IEvent;
namespace Ncqrs.NServiceBus
{
    /// <summary>
    /// A <see cref="IEventBus"/> implementation using NServiceBus. Forwards all published
    /// events via NServiceBus. Does NOT support registering local event handlers using
    /// <see cref="RegisterHandler{TEvent}"/>. All events passed to <see cref="Publish(Ncqrs.Eventing.ServiceModel.Bus.IPublishedEvent{Ncqrs.Eventing.ServiceModel.Bus.IPublishableEvent})"/>
    /// method are send using of NServiceBus transport level message.
    /// </summary>
    public class NsbEventBus : IEventBus
    {
        public void Publish(IPublishableEvent eventMessage)
        {
            Bus.Publish(CreateEventMessage(eventMessage));
        }

        public void Publish(IEnumerable<IPublishableEvent> eventMessages)
        {
            foreach (var msg in eventMessages) { 
                Bus.Publish(CreateEventMessage(msg));
            }
        }

        public void RegisterHandler<TEvent>(IEventHandler<TEvent> handler)
        {
            throw new NotSupportedException("Registering local event handlers with NsbEventBus is not supported. Use CompositeEventBus instead.");
        }

        private static IBus Bus
        {
            get { return NcqrsEnvironment.Get<IBus>(); }
        }

        private static IMessage CreateEventMessage(IPublishableEvent evnt)
        {
            var factory = (IEventMessageFactory)Activator.CreateInstance(typeof(EventMessageFactory));
            return factory.CreateEventMessage(evnt);
        }

        public interface IEventMessageFactory
        {
            IEvent CreateEventMessage(IPublishableEvent payload);
        }

        private class EventMessageFactory : IEventMessageFactory
        {
            IEvent IEventMessageFactory.CreateEventMessage(IPublishableEvent evnt)
            {
                return new EventMessage(evnt);
            }
        }
    }
}

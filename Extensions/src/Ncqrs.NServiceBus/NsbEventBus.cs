﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;
using NServiceBus;

namespace Ncqrs.NServiceBus
{
    /// <summary>
    /// A <see cref="IEventBus"/> implementation using NServiceBus. Forwards all published
    /// events via NServiceBus. Does NOT support registering local event handlers using
    /// <see cref="RegisterHandler{TEvent}"/>. All events passed to <see cref="Publish(System.Collections.Generic.IEnumerable{Ncqrs.Eventing.IEvent})"/>
    /// method are send using of NServiceBus transport level message.
    /// </summary>
    public class NsbEventBus : IEventBus
    {
        public virtual void Publish(IEvent eventMessage)
        {
            Bus.Publish(CreateEventMessage(eventMessage));
        }

        public virtual void Publish(IEnumerable<IEvent> eventMessages)
        {
            Bus.Publish(eventMessages.Select(CreateEventMessage).ToArray());
        }

        public void RegisterHandler<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent
        {
            throw new NotSupportedException("Registering local event handlers with NsbEventBus is not supported yet.");
        }

        protected static IBus Bus
        {
            get { return NcqrsEnvironment.Get<IBus>(); }
        }

        protected static IMessage CreateEventMessage(IEvent payload)
        {
            Type factoryType =
               typeof(EventMessageFactory<>).MakeGenericType(payload.GetType());
            var factory =
               (IEventMessageFactory)Activator.CreateInstance(factoryType);
            return factory.CreateEventMessage(payload);
        }

        public interface IEventMessageFactory
        {
            IMessage CreateEventMessage(IEvent payload);
        }

        protected class EventMessageFactory<T> : IEventMessageFactory where T : IEvent
        {
            IMessage IEventMessageFactory.CreateEventMessage(IEvent payload)
            {
                return new EventMessage<T>
                          {
                              Payload = (T)payload
                          };
            }
        }
    }
}

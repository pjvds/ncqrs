using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Windsor;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;
using Castle.Core;

namespace Ncqrs.Config.Windsor
{
    public class WindsorInProcessEventBus : IEventBus
    {
        static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        readonly IWindsorContainer _container;

        public WindsorInProcessEventBus(IWindsorContainer container)
        {
            _container = container;
        }


        public void Publish(IPublishableEvent eventMessage)
        {
            var eventMessageType = eventMessage.GetType();

            Log.InfoFormat("Started publishing event {0}.", eventMessageType.FullName);

            var handlers = GetHandlersForEvent(eventMessage).ToList();

            var publishedEventClosedType = typeof(PublishedEvent<>).MakeGenericType(eventMessage.Payload.GetType());
            var publishedEvent = (PublishedEvent)Activator.CreateInstance(publishedEventClosedType, eventMessage);

            if (handlers.Count() == 0)
            {
                Log.WarnFormat("Did not found any handlers for event {0}.", eventMessageType.FullName);
            }
            else
            {
                PublishToHandlers(publishedEvent, eventMessageType, handlers);
            }
        }

        public void Publish(IEnumerable<IPublishableEvent> eventMessages)
        {
            eventMessages.ForEach(Publish);
        }
        
        static void PublishToHandlers(dynamic eventMessage, Type eventMessageType, IEnumerable<dynamic> handlers)
        {
            Log.DebugFormat("Found {0} handlers for event {1}.", handlers.Count(), eventMessageType.FullName);

            foreach (var handler in handlers)
            {
                Log.DebugFormat("Calling handler {0} for event {1}.", handler.GetType().FullName,
                    eventMessageType.FullName);

                handler.Handle(eventMessage);

                Log.DebugFormat("Call finished.");
            }
        }

        protected virtual IEnumerable<dynamic> GetHandlersForEvent(IPublishableEvent eventMessage)
        {
            var type = eventMessage.Payload.GetType();
            var handlerType = typeof (IEventHandler<>).MakeGenericType(type);
            var handlers = _container.ResolveAll(handlerType);
            return handlers.Cast<dynamic>();
        }
    }
}
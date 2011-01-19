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
        readonly IWindsorContainer _container;
        public WindsorInProcessEventBus(IWindsorContainer container) { _container = container; }

        static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public virtual void Publish(IEvent eventMessage)
        {
            var eventMessageType = eventMessage.GetType();

            Log.InfoFormat("Started publishing event {0}.", eventMessageType.FullName);

            var handlers = GetHandlersForEvent(eventMessage);

            if (handlers.Count() == 0)
                Log.WarnFormat("Did not found any handlers for event {0}.", eventMessageType.FullName);
            else
                PublishToHandlers(eventMessage, eventMessageType, handlers);
        }

        public virtual void Publish(IEnumerable<IEvent> eventMessages)
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

        protected virtual IEnumerable<dynamic> GetHandlersForEvent(IEvent eventMessage)
        {
            var eventTypes = new List<Type>();
            var type = eventMessage.GetType();
            while (type != null)
            {
                var interfaces = type.GetInterfaces().Where(i => typeof (IEvent).IsAssignableFrom(i));
                eventTypes.Add(type);
                eventTypes.AddRange(interfaces);
                type = typeof (IEvent).IsAssignableFrom(type.BaseType) ? type.BaseType : null;
            }
            eventTypes = eventTypes.Distinct().ToList();
            foreach (var eventType in eventTypes)
            {
                var handlerType = typeof (IEventHandler<>).MakeGenericType(eventType);
                var handlers = _container.ResolveAll(handlerType);
                foreach (var handler in handlers)
                    yield return handler;
            }
        }
    }
}
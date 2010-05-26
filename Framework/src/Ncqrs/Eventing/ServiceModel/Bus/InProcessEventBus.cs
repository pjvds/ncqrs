using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;

namespace Ncqrs.Eventing.ServiceModel.Bus
{
    public class InProcessEventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Action<IEvent>>> _handlerRegister = new Dictionary<Type, List<Action<IEvent>>>();
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Publish(IEvent eventMessage)
        {
            var eventMessageType = eventMessage.GetType();

            Log.InfoFormat("Started publishing event {0}.", eventMessageType.FullName);

            IEnumerable<Action<IEvent>> handlers = GetHandlersForEvent(eventMessage);

            if (handlers.Count() == 0)
            {
                Log.WarnFormat("Did not found any handlers for event {0}.", eventMessageType.FullName);
            }
            else
            {
                using (var transaction = new TransactionScope())
                {
                    Log.DebugFormat("Found {0} handlers for event {1}.", handlers.Count(), eventMessageType.FullName);

                    foreach (var handler in handlers)
                    {
                        Log.DebugFormat("Calling handler {0} for event {1}.", handler.GetType().FullName,
                                        eventMessageType.FullName);

                        handler(eventMessage);

                        Log.DebugFormat("Call finished.");
                    }
                    transaction.Complete();
                }
            }
        }

        protected IEnumerable<Action<IEvent>> GetHandlersForEvent(IEvent eventMessage)
        {
            var eventType = eventMessage.GetType();
            var result = new List<Action<IEvent>>();

            foreach(var key in _handlerRegister.Keys)
            {
                if(key.IsAssignableFrom(eventType))
                {
                    var handlers = _handlerRegister[key];
                    result.AddRange(handlers);
                }
            }

            return result;
        }

        public void Publish(IEnumerable<IEvent> eventMessages)
        {
            foreach (var eventMessage in eventMessages)
            {
                Publish(eventMessage);
            }
        }

        public void RegisterHandler<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent
        {
            Action<IEvent> act = (e) => handler.Handle((TEvent) e);
            RegisterHandler(typeof(TEvent), act);
        }

        public void RegisterHandler(Type eventType, Action<IEvent> handler)
        {
            List<Action<IEvent>> handlers = null;
            if (!_handlerRegister.TryGetValue(eventType, out handlers))
            {
                handlers = new List<Action<IEvent>>(1);
                _handlerRegister.Add(eventType, handlers);
            }

            handlers.Add(handler);
        }
    }
}
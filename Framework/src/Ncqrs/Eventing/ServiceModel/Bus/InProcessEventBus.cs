using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;

namespace Ncqrs.Eventing.ServiceModel.Bus
{
    public class InProcessEventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Action<IEvent<IEventData>>>> _handlerRegister = new Dictionary<Type, List<Action<IEvent<IEventData>>>>();
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool _useTransactionScope;

        /// <summary>
        /// Creates new <see cref="InProcessEventBus"/> instance that wraps publishing to
        /// handlers into a <see cref="TransactionScope"/>.
        /// </summary>
        public InProcessEventBus()
            : this(true)
        {            
        }

        /// <summary>
        /// Creates new <see cref="InProcessEventBus"/> instance.
        /// </summary>
        /// <param name="useTransactionScope">Use transaction scope?</param>
        public InProcessEventBus(bool useTransactionScope)            
        {
            _useTransactionScope = useTransactionScope;
        }

        public void Publish(IEvent<IEventData> eventMessage)
        {
            var eventMessageType = eventMessage.GetType();

            Log.InfoFormat("Started publishing event {0}.", eventMessageType.FullName);

            IEnumerable<Action<IEvent<IEventData>>> handlers = GetHandlersForEvent(eventMessage);

            if (handlers.Count() == 0)
            {
                Log.WarnFormat("Did not found any handlers for event {0}.", eventMessageType.FullName);
            }
            else
            {
                if (_useTransactionScope)
                {
                    TransactionallyPublishToHandlers(eventMessage, eventMessageType, handlers);
                }
                else
                {
                    PublishToHandlers(eventMessage, eventMessageType, handlers);
                }
            }
        }

        private static void TransactionallyPublishToHandlers(IEvent<IEventData> eventMessage, Type eventMessageType, IEnumerable<Action<IEvent<IEventData>>> handlers)
        {
            using (var transaction = new TransactionScope())
            {
                PublishToHandlers(eventMessage, eventMessageType, handlers);
                transaction.Complete();
            }
        }

        private static void PublishToHandlers(IEvent<IEventData> eventMessage, Type eventMessageType, IEnumerable<Action<IEvent<IEventData>>> handlers)
        {
            Log.DebugFormat("Found {0} handlers for event {1}.", handlers.Count(), eventMessageType.FullName);

            foreach (var handler in handlers)
            {
                Log.DebugFormat("Calling handler {0} for event {1}.", handler.GetType().FullName,
                                eventMessageType.FullName);

                handler(eventMessage);

                Log.DebugFormat("Call finished.");
            }
        }

        protected IEnumerable<Action<IEvent<IEventData>>> GetHandlersForEvent(IEvent<IEventData> eventMessage)
        {
            var eventType = eventMessage.GetType();
            var result = new List<Action<IEvent<IEventData>>>();

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

        public void Publish(IEnumerable<IEvent<IEventData>> eventMessages)
        {
            foreach (var eventMessage in eventMessages)
            {
                Publish(eventMessage);
            }
        }

        public void RegisterHandler<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
        {
            Action<IEvent<IEventData>> act = (e) => handler.Handle((IEvent<TEventData>)e);
            RegisterHandler(typeof(IEvent<TEventData>), act);
        }

        public void RegisterHandler(Type eventType, Action<IEvent<IEventData>> handler)
        {
            List<Action<IEvent<IEventData>>> handlers = null;
            if (!_handlerRegister.TryGetValue(eventType, out handlers))
            {
                handlers = new List<Action<IEvent<IEventData>>>(1);
                _handlerRegister.Add(eventType, handlers);
            }

            handlers.Add(handler);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Transactions;
using log4net;
using System.Reflection;

namespace Ncqrs.Eventing.Bus
{
    public class InProcessEventBus : IEventBus
    {
        private readonly Dictionary<Type, List<IEventHandler>> _handlerRegister = new Dictionary<Type, List<IEventHandler>>();
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Publish(IEvent eventMessage)
        {
            Contract.Requires<ArgumentNullException>(eventMessage != null);
            var eventMessageType = eventMessage.GetType();

            Log.InfoFormat("Started publishing event {0}.", eventMessageType.FullName);

            using (var transaction = new TransactionScope())
            {
                List<IEventHandler> handlers = null;

                if(!_handlerRegister.TryGetValue(eventMessageType, out handlers))
                {
                    Log.WarnFormat("Did not found any handlers for event {0}.", eventMessageType.FullName);
                }
                else
                {
                    Log.DebugFormat("Found {0} handlers for event {1}.", handlers.Count, eventMessageType.FullName);

                    foreach (var handler in handlers)
                    {
                        Log.DebugFormat("Calling handler {0} for event {1}.", handler.GetType().FullName, eventMessageType.FullName);

                        handler.Handle(eventMessage);

                        Log.DebugFormat("Call finished.");
                    }
                }

                transaction.Complete();
            }
        }

        public void Publish(IEnumerable<IEvent> eventMessages)
        {
            foreach (var eventMessage in eventMessages)
            {
                Publish(eventMessage);
            }
        }

        public void RegisterHandler<TEvent>(IEventHandler handler) where TEvent : IEvent
        {
            RegisterHandler(typeof(TEvent), handler);
        }

        public void RegisterHandler(Type eventType, IEventHandler handler)
        {
            Contract.Requires<ArgumentNullException>(eventType != null);
            Contract.Requires<ArgumentNullException>(handler != null);

            List<IEventHandler> handlers = null;
            if (!_handlerRegister.TryGetValue(eventType, out handlers))
            {
                handlers = new List<IEventHandler>(1);
                _handlerRegister.Add(eventType, handlers);
            }

            handlers.Add(handler);
        }
    }
}
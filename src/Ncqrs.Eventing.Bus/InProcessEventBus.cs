using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Transactions;

namespace Ncqrs.Eventing.Bus
{
    public class InProcessEventBus : IEventBus
    {
        private readonly Dictionary<Type, List<IEventHandler>> _handlerRegister = new Dictionary<Type, List<IEventHandler>>();

        public void Publish(IEvent eventMessage)
        {
            Contract.Requires<ArgumentNullException>(eventMessage != null);

            using (var transaction = new TransactionScope())
            {
                var eventMessageType = eventMessage.GetType();
                var handlers = _handlerRegister[eventMessageType];

                foreach (var handler in handlers)
                {
                    handler.Handle(eventMessage);
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

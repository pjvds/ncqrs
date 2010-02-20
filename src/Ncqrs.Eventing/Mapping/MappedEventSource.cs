using System;
using System.Collections.Generic;

namespace Ncqrs.Eventing.Mapping
{
    public abstract class MappedEventSource : EventSource
    {
        private readonly Dictionary<Type, Action<IEvent>> _handlers = new Dictionary<Type, Action<IEvent>>(0);

        protected MappedEventSource(IUniqueIdentifierGenerator idGenerator) : base(idGenerator)
        {
            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            var handlerFactory = new EventHandlerFactory();
            foreach (var x in handlerFactory.CreateHandlers(this))
            {
                RegisterHandler(x.Key, x.Value);
            }
        }

        protected void RegisterHandler<T>(Action<T> handler) where T : IEvent
        {
            if (handler == null) throw new ArgumentNullException("handler");
            var eventType = typeof(T);

            RegisterHandler(eventType, (evnt) => handler((T)evnt));
        }
        
        protected void RegisterHandler(Type eventType, Action<IEvent> handler)
        {
            if(eventType == null) throw new ArgumentNullException("eventType");
            if(handler == null) throw new ArgumentNullException("handler");
            if (_handlers.ContainsKey(eventType)) throw new EventAlreadyHandledException("");// TODO: More details.

            _handlers.Add(eventType, handler);
        }

        protected override void HandleEvent(IEvent evnt)
        {
            if(evnt == null) throw new ArgumentNullException("event");

            var handler = GetHandlerForEvent(evnt);
            handler(evnt);
        }

        private Action<IEvent> GetHandlerForEvent(IEvent evnt)
        {
            Action<IEvent> handler;
            Type eventType = evnt.GetType();

            if (!_handlers.TryGetValue(eventType, out handler))
            {
                throw new NoEventHandlerFoundException(evnt);
            }

            return handler;
        }
    }
}

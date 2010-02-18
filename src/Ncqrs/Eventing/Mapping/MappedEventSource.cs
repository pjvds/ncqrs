using System;
using System.Collections.Generic;

namespace Ncqrs.Eventing.Mapping
{
    public abstract class MappedEventSource : EventSource
    {
        private readonly Dictionary<Type, IEventHandler> _handlers = new Dictionary<Type, IEventHandler>(0);

        protected MappedEventSource() : base()
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

        protected void RegisterHandler(Type eventType, IEventHandler handler)
        {
            if(eventType == null) throw new ArgumentNullException("eventType");
            if(handler == null) throw new ArgumentNullException("handler");
            if (_handlers.ContainsKey(eventType)) throw new EventAlreadyHandledException("");// TODO: More details.

            _handlers.Add(eventType, handler);
        }

        protected override void HandleEvent(IEvent evnt)
        {
            if(evnt == null) throw new ArgumentNullException("event");

            IEventHandler handler = GetHandlerForEvent(evnt);
            handler.Invoke(evnt);
        }

        private IEventHandler GetHandlerForEvent(IEvent evnt)
        {
            IEventHandler handler;
            Type eventType = evnt.GetType();

            if (!_handlers.TryGetValue(eventType, out handler))
            {
                throw new NoEventHandlerFoundException(evnt);
            }

            return handler;
        }
    }
}

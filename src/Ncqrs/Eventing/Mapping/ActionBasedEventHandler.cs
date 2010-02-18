using System;

namespace Ncqrs.Eventing.Mapping
{
    public class ActionBasedEventHandler : IEventHandler
    {
        private readonly Action<IEvent> _action;

        public ActionBasedEventHandler(Action<IEvent> action)
        {
            if(action == null) throw new ArgumentNullException("action");

            _action = action;
        }

        public void Invoke(IEvent evnt)
        {
            _action(evnt);
        }
    }
}
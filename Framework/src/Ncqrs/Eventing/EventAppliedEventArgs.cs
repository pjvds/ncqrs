using System;
using System.Diagnostics.Contracts;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Eventing
{
    public class EventAppliedEventArgs : EventArgs
    {
        public IEvent Event
        {
            get;
            private set;
        }

        public EventAppliedEventArgs(IEvent evnt)
        {
            Contract.Requires<ArgumentNullException>(evnt != null);
            Contract.Ensures(Event == evnt);

            Event = evnt;
        }
    }
}

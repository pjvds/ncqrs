using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

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

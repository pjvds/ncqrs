using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Ncqrs.Domain
{
   public class EventsAcceptedEventArgs : EventArgs
    {
       public IEnumerable<IEvent> AcceptedEvents
       {
           get;
           private set;
       }

       public EventsAcceptedEventArgs(IEnumerable<IEvent> acceptedEvents)
       {
           Contract.Requires<ArgumentNullException>(acceptedEvents != null);
           Contract.Ensures(AcceptedEvents == acceptedEvents);

           AcceptedEvents = acceptedEvents;
       }
    }
}

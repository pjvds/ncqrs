using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Eventing
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

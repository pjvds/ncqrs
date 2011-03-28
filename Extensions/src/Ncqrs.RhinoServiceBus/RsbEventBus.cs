using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.ServiceModel;
using Ncqrs.Eventing.ServiceModel.Bus;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Hosting;

namespace Ncqrs.RhinoServiceBus
{
    class RsbEventBus : IEventBus
    {
        public void Publish(IEnumerable<IPublishableEvent> eventMessages)
        {
            Bus.Publish(eventMessages.ToArray());
        }

        public void Publish(IPublishableEvent eventMessage)
        {
            Bus.Publish(eventMessage);
        }

        private static IServiceBus Bus
        {
            get { return NcqrsEnvironment.Get<IServiceBus>(); }
        }
    }
}
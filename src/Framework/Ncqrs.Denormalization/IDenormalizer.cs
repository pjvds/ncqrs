using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Bus;

namespace Ncqrs.Denormalization
{
    public interface IDenormalizer : IEventHandler
    {
        void DenormalizeEvent(IEvent evnt);        
    }
}

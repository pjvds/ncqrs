using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain;
using Ncqrs.Domain.Bus;

namespace Ncqrs.Denormalization
{
    public interface IDenormalizer : IEventHandler
    {
        void DenormalizeEvent(IEvent evnt);        
    }
}

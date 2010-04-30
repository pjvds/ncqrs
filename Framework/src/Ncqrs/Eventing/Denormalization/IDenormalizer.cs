using System;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Eventing.Denormalization
{
    public interface IDenormalizer : IEventHandler
    {
        void DenormalizeEvent(IEvent evnt);        
    }
}

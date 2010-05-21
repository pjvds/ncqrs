using System;

namespace Ncqrs.Eventing.Denormalization
{
    public interface IDenormalizer<TEvent>
    {
        void DenormalizeEvent(TEvent evnt);        
    }
}
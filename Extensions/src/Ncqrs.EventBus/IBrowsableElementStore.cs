using System.Collections.Generic;

namespace Ncqrs.EventBus
{    
    public interface IBrowsableElementStore
    {
        IEnumerable<IProcessingElement> Fetch(int maxCount);
        void MarkLastProcessedEvent(IProcessingElement processingElement);
    }
}

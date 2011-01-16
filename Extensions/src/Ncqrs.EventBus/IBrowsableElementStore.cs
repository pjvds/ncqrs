using System.Collections.Generic;

namespace Ncqrs.EventBus
{    
    public interface IBrowsableElementStore
    {
        IEnumerable<IProcessingElement> Fetch(string pipelineName, int maxCount);
        void MarkLastProcessedEvent(string pipelineName, IProcessingElement processingElement);
    }
}

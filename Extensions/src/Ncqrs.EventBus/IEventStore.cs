using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ncqrs.EventBus
{    
    public interface IEventStore
    {
        void SetCursorPositionAfter(Guid lastEventId);
        SequencedEvent GetNext();
        void UnblockSource(Guid eventSourceId);
    }
}

using System;

namespace Ncqrs.Eventing
{
    public class Snapshot : ISnapshot
    {
        public IMemento Memento
        { 
            get; private set;
        }

        public Guid EventSourceId
        {
            get; private set;
        }

        public long EventSourceVersion
        {
            get; private set;
        }

        public Snapshot(Guid eventSourceId, long eventSourceVersion, IMemento memento)
        {
            EventSourceId = eventSourceId;
            EventSourceVersion = eventSourceVersion;
            Memento = memento;
        }
    }
}

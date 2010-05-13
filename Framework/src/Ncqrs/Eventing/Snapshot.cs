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

        public Snapshot(IMemento memento, Guid eventSourceId, long eventSourceVersion)
        {
            Memento = memento;
            EventSourceId = eventSourceId;
            EventSourceVersion = eventSourceVersion;
        }
    }
}

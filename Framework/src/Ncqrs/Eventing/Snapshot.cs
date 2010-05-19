using System;

namespace Ncqrs.Eventing
{
    public abstract class SnapshotBase : ISnapshot
    {
        public Guid EventSourceId
        {
            get; private set;
        }

        public long EventSourceVersion
        {
            get; private set;
        }

        public SnapshotBase(Guid eventSourceId, long eventSourceVersion)
        {
            EventSourceId = eventSourceId;
            EventSourceVersion = eventSourceVersion;
        }
    }
}

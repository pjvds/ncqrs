using System;

namespace Ncqrs.Eventing
{
    [Serializable]
    public abstract class Snapshot : ISnapshot
    {
        public Guid EventSourceId
        {
            get; set;
        }

        public long EventSourceVersion
        {
            get; set;
        }
    }
}

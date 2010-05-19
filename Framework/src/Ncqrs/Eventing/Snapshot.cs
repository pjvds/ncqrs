using System;

namespace Ncqrs.Eventing
{
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

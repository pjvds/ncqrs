using System;
using System.Runtime.Serialization;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{

    [Serializable]
    public class DynamicSnapshotNotSupportedException : DynamicSnapshotException
    {
        public Type AggregateType { get; set; }

        public DynamicSnapshotNotSupportedException()
        {

        }
        public DynamicSnapshotNotSupportedException(string message, Exception inner)
            : base(message, inner)
        {

        }
        public DynamicSnapshotNotSupportedException(string message)
            : base(message)
        {

        }

        protected DynamicSnapshotNotSupportedException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }
    }
}
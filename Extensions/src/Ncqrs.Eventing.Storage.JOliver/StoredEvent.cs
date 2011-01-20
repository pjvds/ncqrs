using System;
using System.Runtime.Serialization;

namespace Ncqrs.Eventing.Storage.JOliver
{
    [Serializable]
    [DataContract]
    public class StoredEvent
    {
        [DataMember]
        public DateTime TimeStamp { get; set; }
        [DataMember]
        public long Sequence { get; set; }
        [DataMember]
        public object Body { get; set; }
        [DataMember]
        public Guid CommitId { get; set; }
        [DataMember]
        public Guid EventId { get; set; }
        [DataMember]
        public int MajorVersion { get; set; }
        [DataMember]
        public int MinorVersion { get; set; }
    }
}
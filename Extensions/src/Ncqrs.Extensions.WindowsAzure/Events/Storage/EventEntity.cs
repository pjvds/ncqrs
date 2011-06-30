using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using Ncqrs.Eventing;

namespace Ncqrs.Extensions.WindowsAzure.Events.Storage {
    public class EventEntity : TableServiceEntity {
        public EventEntity(UncommittedEvent uncomittedEvent) : base(uncomittedEvent.EventSourceId.ToString(),
            Utility.GetRowKey(uncomittedEvent.EventSequence)) {
            _commitId = uncomittedEvent.CommitId;
            _eventIdentifier = uncomittedEvent.EventIdentifier;
            _eventSequence = uncomittedEvent.EventSequence;
            _eventSourceId = uncomittedEvent.EventSourceId;
            _eventTimeStamp = uncomittedEvent.EventTimeStamp;
            _eventVersion = uncomittedEvent.EventVersion.ToString();
            _name = uncomittedEvent.Payload.GetType().AssemblyQualifiedName;
            _payload = Utility.Jsonize(uncomittedEvent.Payload, uncomittedEvent.Payload.GetType());
        }
        public EventEntity() {
        }

        string _name;

        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        string _payload;

        public string Payload {
            get { return _payload; }
            set { _payload = value; }
        }
        string _eventVersion;

        public string EventVersion {
            get { return _eventVersion; }
            set { _eventVersion = value; }
        }
        Guid _commitId;

        public Guid CommitId {
            get { return _commitId; }
            set { _commitId = value; }
        }
        Guid _eventSourceId;

        public Guid EventSourceId {
            get { return _eventSourceId; }
            set { _eventSourceId = value; }
        }
        long _eventSequence;

        public long EventSequence {
            get { return _eventSequence; }
            set { _eventSequence = value; }
        }
        Guid _eventIdentifier;

        public Guid EventIdentifier {
            get { return _eventIdentifier; }
            set { _eventIdentifier = value; }
        }
        DateTime _eventTimeStamp;

        public DateTime EventTimeStamp {
            get { return _eventTimeStamp; }
            set { _eventTimeStamp = value; }
        }
    }
}

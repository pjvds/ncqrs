using System;
using System.Runtime.Serialization;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>
    /// Occurs when rying to save commit which is already stored in the event store. Usually it means that
    /// the same command has been executed more than onece. It is up to command handler to swallow it possibly
    /// putting some into in the logs.
    /// </summary>
    [Serializable]
    public class DuplicateCommitException : Exception
    {
        private readonly Guid _eventSourceId;
        private readonly Guid _commitId;

        public DuplicateCommitException(Guid eventSourceId, Guid commitId)
        {
            _eventSourceId = eventSourceId;
            _commitId = commitId;
        }


        protected DuplicateCommitException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _commitId = (Guid) info.GetValue("CommitId", typeof (Guid));
            _eventSourceId = (Guid)info.GetValue("EventSourceId", typeof(Guid));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("CommitId", _commitId);
            info.AddValue("EventSourceId", _eventSourceId);
        }

        public Guid CommitId
        {
            get { return _commitId; }
        }

        public Guid EventSourceId
        {
            get { return _eventSourceId; }
        }


    }
}
using EventStore;
using Ncqrs.EventBus;

namespace Ncqrs.Eventing.Storage.JOliver
{
    public class CommitProcessingElement : IProcessingElement
    {
        private readonly Commit _commit;

        public CommitProcessingElement(Commit commit)
        {
            _commit = commit;
        }

        public Commit Commit
        {
            get { return _commit; }
        }

        public int SequenceNumber { get; set; }

        public string UniqueId
        {
            get { return string.Format("[{0},{1}]", _commit.StreamId, _commit.CommitId); }
        }

        public object GroupingKey
        {
            get { return _commit.StreamId; }
        }
    }
}
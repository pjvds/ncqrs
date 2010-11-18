using System;

namespace Ncqrs.EventBus
{
    public struct FetchResult
    {
        private readonly Guid _uniqueId;
        private readonly int _maxCount;
        private readonly int _fetchedCount;
        private readonly TimeSpan _duration;

        public FetchResult(Guid uniqueId, int maxCount, int fetchedCount, TimeSpan duration) : this()
        {
            _uniqueId = uniqueId;
            _duration = duration;
            _fetchedCount = fetchedCount;
            _maxCount = maxCount;
        }

        public TimeSpan Duration
        {
            get { return _duration; }
        }

        public int FetchedCount
        {
            get { return _fetchedCount; }
        }

        public int MaxCount
        {
            get { return _maxCount; }
        }

        public Guid UniqueId
        {
            get { return _uniqueId; }
        }
    }
}
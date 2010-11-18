using System;

namespace Ncqrs.EventBus
{
    public struct FetchDirective
    {
        private readonly bool _shouldFetch;
        private readonly int _maxCount;
        private readonly Guid? _uniqueId;

        private FetchDirective(bool shouldFetch, int maxCount, Guid? uniqueId)
        {
            _shouldFetch = shouldFetch;
            _uniqueId = uniqueId;
            _maxCount = maxCount;
        }

        public Guid? UniqueId
        {
            get { return _uniqueId; }
        }

        public static FetchDirective FetchNow(Guid uniqueId, int maxCount)
        {
            return new FetchDirective(true, maxCount, uniqueId);
        }

        public static FetchDirective DoNotFetchYet()
        {
            return new FetchDirective(false, 0, null);
        }

        public bool ShouldFetch
        {
            get { return _shouldFetch; }
        }

        public int MaxCount
        {
            get { return _maxCount; }
        }
    }
    
}
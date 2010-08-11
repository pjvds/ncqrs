namespace Ncqrs.EventBus
{
    public struct FetchDirective
    {
        private readonly bool _shouldFetch;
        private readonly int _maxCount;

        private FetchDirective(bool shouldFetch, int maxCount)
        {
            _shouldFetch = shouldFetch;
            _maxCount = maxCount;
        }

        public static FetchDirective FetchNow(int maxCount)
        {
            return new FetchDirective(true, maxCount);
        }

        public static FetchDirective DoNotFetchYet()
        {
            return new FetchDirective(false, 0);
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
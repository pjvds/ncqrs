using System;

namespace Ncqrs.Eventing
{
    /// <summary>
    /// Contains information about an event source.
    /// </summary>
    public class EventSourceInformation
    {
        private readonly Guid _id;
        private readonly long _initialVersion;
        private readonly long _currentVersion;

        public EventSourceInformation(Guid id, long initialVersion, long currentVersion)
        {
            _id = id;
            _currentVersion = currentVersion;
            _initialVersion = initialVersion;
        }

        public long CurrentVersion
        {
            get { return _currentVersion; }
        }

        public long InitialVersion
        {
            get { return _initialVersion; }
        }

        public Guid Id
        {
            get { return _id; }
        }
    }
}
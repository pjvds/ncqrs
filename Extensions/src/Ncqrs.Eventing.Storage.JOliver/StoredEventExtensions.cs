using System;

namespace Ncqrs.Eventing.Storage.JOliver
{
    internal static class StoredEventExtensions
    {
        public static CommittedEvent Convert(this StoredEvent x, Guid sourceId)
        {
            return new CommittedEvent(x.CommitId, x.EventId, sourceId, x.Sequence, x.TimeStamp, x.Body,
                                      new Version(x.MajorVersion, x.MinorVersion));
        }
    }
}
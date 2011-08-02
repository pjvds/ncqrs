using System;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    /// <summary>
    /// Excludes a field from the snapshot.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ExcludeFromSnapshotAttribute : Attribute
    {
    }
}

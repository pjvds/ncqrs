using System;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    /// <summary>
    /// Marks the aggregate as ISnapshotable<>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DynamicSnapshotAttribute : Attribute
    {
    }
}

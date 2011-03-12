using System;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ExcludeFromSnapshotAttribute : Attribute
    {
    }
}

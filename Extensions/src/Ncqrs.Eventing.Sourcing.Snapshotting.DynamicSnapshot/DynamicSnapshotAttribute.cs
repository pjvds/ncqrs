using System;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DynamicSnapshotAttribute : Attribute
    {
    }
}

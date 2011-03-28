using System;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    public static class SnapshotNameGenerator
    {
        public static string Generate(Type sourceType)
        {
            return string.Format("{0}_Snapshot", sourceType.Name);
        }
    }
}

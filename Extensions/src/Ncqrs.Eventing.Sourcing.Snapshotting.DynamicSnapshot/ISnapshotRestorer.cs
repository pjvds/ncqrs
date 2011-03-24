using System;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    internal interface ISnapshotRestorer
    {
        bool Restore();
    }
}

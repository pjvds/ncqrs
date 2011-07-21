using System;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    public interface ISnapshotableImplementer<TSnapshot> : ISnapshotable<TSnapshot>, IHaveProxyReference
            where TSnapshot : DynamicSnapshotBase
    {
    }
}

using System;
using System.Linq;
using Ncqrs.Domain;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    internal class SnapshotableImplementer<TSnapshot> : ISnapshotable<TSnapshot>, IHaveProxyReference
            where TSnapshot : DynamicSnapshotBase
    {
        public object Proxy { get; set; }

        public TSnapshot CreateSnapshot()
        {
            return (TSnapshot)Activator.CreateInstance(typeof(TSnapshot), Proxy);
        }

        public void RestoreFromSnapshot(TSnapshot snapshot)
        {
            snapshot.RestoreAggregateRoot((AggregateRoot)Proxy);
        }
    }
}

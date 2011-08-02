using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain;
using System.Reflection;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    internal class SnapshotRestorer<TSnapshot> : ISnapshotRestorer where TSnapshot : DynamicSnapshotBase
    {
        private readonly AggregateRoot _aggregateRoot;
        private readonly TSnapshot _snapshot;
        private readonly MethodInfo _restore;

        public SnapshotRestorer(AggregateRoot aggregateRoot, object snapshot)
        {
            _aggregateRoot = aggregateRoot;
            _snapshot = (TSnapshot)snapshot;
            _restore = aggregateRoot.GetType().GetMethod("RestoreFromSnapshot");
        }

        public bool Restore()
        {
            if (_restore != null)
            {
                _restore.Invoke(_aggregateRoot, new TSnapshot[] { _snapshot });
                return true;
            }
            return false;
        }
    }
}
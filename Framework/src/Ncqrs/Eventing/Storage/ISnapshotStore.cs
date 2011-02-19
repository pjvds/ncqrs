using System;
using System.Diagnostics.Contracts;
using Ncqrs.Eventing.Sourcing.Snapshotting;

namespace Ncqrs.Eventing.Storage
{
    [ContractClass(typeof(ISnapshotStoreContracts))]
    public interface ISnapshotStore
    {
        /// <summary>
        /// Saves a snapshot of the specified event source.
        /// </summary>
        void SaveShapshot(Snapshot source);

        /// <summary>
        /// Gets a snapshot of a particular event source, if one exists. Otherwise, returns <c>null</c>.
        /// </summary>
        Snapshot GetSnapshot(Guid eventSourceId, long maxVersion);
    }

    [ContractClassFor(typeof(ISnapshotStore))]
    internal abstract class ISnapshotStoreContracts : ISnapshotStore
    {
        public void SaveShapshot(Snapshot source)
        {
            Contract.Requires<ArgumentNullException>(source != null, "The source cannot be null.");
        }

        public Snapshot GetSnapshot(Guid eventSourceId, long maxVersion)
        {
            Contract.Ensures(Contract.Result<Snapshot>() != null ? Contract.Result<Snapshot>().EventSourceId == eventSourceId : true);

            return default(Snapshot);
        }
    }
}

using System;
using System.Diagnostics.Contracts;

using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Sourcing.Snapshotting;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>A <see cref="Snapshot"/> store. Can store and retrieve a <see cref="Snapshot"/>.</summary>
    [ContractClass(typeof(ISnapshotStoreContracts))]
    public interface ISnapshotStore
    {
        /// <summary>Gets a snapshot of a particular event source, if one exists. Otherwise, returns <c>null</c>.</summary>
        /// <param name="eventSourceId">Indicates the event source to retrieve the snapshot for.</param>
        /// <param name="maxVersion">Indicates the maximum allowed version to be returned.</param>
        /// <returns>Returns the most recent <see cref="Snapshot"/> that exists in the store. If the store has a 
        /// snapshot that is more recent than the <paramref name="maxVersion"/>, then <c>null</c> will be returned.</returns>
        Snapshot GetSnapshot(Guid eventSourceId, long maxVersion);

        /// <summary>Persists a <see cref="Snapshot"/> of an <see cref="EventSource"/>.</summary>
        /// <param name="snapshot">The <see cref="Snapshot"/> that is being saved.</param>
        void SaveSnapshot(Snapshot snapshot);
    }

    [ContractClassFor(typeof(ISnapshotStore))]
    internal abstract class ISnapshotStoreContracts : ISnapshotStore
    {
        public Snapshot GetSnapshot(Guid eventSourceId, long maxVersion)
        {
            Contract.Ensures(Contract.Result<Snapshot>() != null ? Contract.Result<Snapshot>().EventSourceId == eventSourceId : true);

            return default(Snapshot);
        }

        public void SaveSnapshot(Snapshot source)
        {
            Contract.Requires<ArgumentNullException>(source != null, "The source cannot be null.");
        }
    }
}

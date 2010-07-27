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
        void SaveShapshot(ISnapshot source);

        /// <summary>
        /// Gets a snapshot of a particular event source, if one exists. Otherwise, returns <c>null</c>.
        /// </summary>
        ISnapshot GetSnapshot(Guid eventSourceId);
    }

    [ContractClassFor(typeof(ISnapshotStore))]
    internal abstract class ISnapshotStoreContracts : ISnapshotStore
    {
        public void SaveShapshot(ISnapshot source)
        {
            Contract.Requires<ArgumentNullException>(source != null, "The source cannot be null.");
        }

        public ISnapshot GetSnapshot(Guid eventSourceId)
        {
            Contract.Ensures(Contract.Result<ISnapshot>() != null ? Contract.Result<ISnapshot>().EventSourceId == eventSourceId : true);

            return default(ISnapshot);
        }
    }
}

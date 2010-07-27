using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Ncqrs.Eventing.Sourcing.Snapshotting
{    
    /// <summary>
    /// This interface flags an object beeing <i>snapshotable</i>. This means
    /// that the state of the object could be saved to an 
    /// <see cref="ISnapshot"/> object and restored from a the from the same
    /// class. This is used to prevent building <see cref="AggregateRoot"/>'s
    /// from the ground up.
    /// </summary>
    /// <remarks>
    /// Only one <see cref="ISnapshotable{TSnapshot}"/> can be implemented. If
    /// you implement more than one, it will cause an exception when saving this
    /// instance.
    /// </remarks>
    [ContractClass(typeof(SnapshotableContracts<>))]
    public interface ISnapshotable<TSnapshot> : IEventSource where TSnapshot : ISnapshot
    {
        TSnapshot CreateSnapshot();
        void RestoreFromSnapshot(TSnapshot snapshot);
    }

    [ContractClassFor(typeof(ISnapshotable<>))]
    internal abstract class SnapshotableContracts<TSnapshot> : ISnapshotable<TSnapshot> where TSnapshot : ISnapshot
    {
        public void RestoreFromSnapshot(TSnapshot snapshot)
        {
            Contract.Ensures(EventSourceId == snapshot.EventSourceId, "Restoring from snapshot should initialize the Id.");
            Contract.Ensures(InitialVersion == snapshot.EventSourceVersion, "Restoring from snapshot should initialize the initial version.");
        }

        public TSnapshot CreateSnapshot()
        {
            Contract.Ensures(Contract.Result<TSnapshot>().EventSourceId == EventSourceId, "The EventSourceId of the snapshot should be initialized with the Id value of the event source.");
            Contract.Ensures(Contract.Result<TSnapshot>().EventSourceVersion == Version, "The EventSourceVersion of the snapshot should be initialized with the Version value of the event source.");

            return default(TSnapshot);
        }

        public Guid EventSourceId
        {
            get { throw new NotImplementedException(); }
        }

        public long Version
        {
            get { throw new NotImplementedException(); }
        }

        public long InitialVersion
        {
            get { throw new NotImplementedException(); }
        }

        public void InitializeFromHistory(IEnumerable<SourcedEvent> history)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SourcedEvent> GetUncommittedEvents()
        {
            throw new NotImplementedException();
        }

        public void AcceptChanges()
        {
            throw new NotImplementedException();
        }
    }
}

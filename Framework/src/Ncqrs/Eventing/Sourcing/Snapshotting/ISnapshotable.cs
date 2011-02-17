using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Ncqrs.Domain;

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
    public interface ISnapshotable<TSnapshot>
    {
        TSnapshot CreateSnapshot();
        void RestoreFromSnapshot(TSnapshot snapshot);
    }

    [ContractClassFor(typeof(ISnapshotable<>))]
    internal abstract class SnapshotableContracts<TSnapshot> : ISnapshotable<TSnapshot>
    {
        public void RestoreFromSnapshot(TSnapshot snapshot)
        {
        }

        public TSnapshot CreateSnapshot()
        {
            return default(TSnapshot);
        }
    }
}

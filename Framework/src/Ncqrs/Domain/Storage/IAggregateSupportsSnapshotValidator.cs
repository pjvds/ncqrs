using System;

namespace Ncqrs.Domain.Storage
{
    public interface IAggregateSupportsSnapshotValidator
    {
        /// <summary>
        /// Ensures that the given aggregate root type supports a particular snapshot.
        /// </summary>
        /// <param name="aggregateType">The type of the aggregate root.</param>
        /// <param name="snapshotType">The type of the snapshot.</param>
        /// <returns></returns>
        bool DoesAggregateSupportsSnapshot(Type aggregateType, Type snapshotType);
    }
}

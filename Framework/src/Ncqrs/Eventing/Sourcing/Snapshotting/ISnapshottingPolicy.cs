using System;
using Ncqrs.Domain;

namespace Ncqrs.Eventing.Sourcing.Snapshotting
{
    /// <summary>
    /// Represents a policy for creating snapshots of aggregate roots.
    /// </summary>
    public interface ISnapshottingPolicy
    {
        /// <summary>
        /// Returns if runtime should attempt to create a snapshot of provided aggregate root at the moment.
        /// </summary>
        /// <param name="aggregateRoot">Aggregate root.</param>
        /// <returns></returns>
        bool ShouldCreateSnapshot(AggregateRoot aggregateRoot);
    }
}
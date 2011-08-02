using System;
using System.Reflection;
using Ncqrs.Domain;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    /// <summary>
    /// Provides a way to create an assembly with snapshot types and finder for snapshot types.
    /// </summary>
    public interface IDynamicSnapshotAssembly
    {
        /// <summary>
        /// Gets the actual assembly.
        /// </summary>
        Assembly ActualAssembly { get; }
        /// <summary>
        /// Finds snapshot type.
        /// </summary>
        /// <param name="aggregateType">Type of the aggregate.</param>
        /// <returns></returns>
        Type FindSnapshotType(Type aggregateType);
        /// <summary>
        /// Finds snapshot type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Type FindSnapshotType<T>()
                where T : AggregateRoot;
        /// <summary>
        /// Creates an assembly with snapshot types.
        /// </summary>
        /// <param name="target">The assembly containing aggregate roots with [DynamicSnapshot] attribute.</param>
        /// <returns></returns>
        Assembly CreateAssemblyFrom(Assembly target);
    }
}

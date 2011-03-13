using System;
using Castle.MicroKernel.Facilities;
using Ncqrs.Domain.Storage;
using Castle.MicroKernel.Registration;
using System.Reflection;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    /// <summary>
    /// Initializes all the infrastructure for DynamicSnapshot.
    /// </summary>
    public class DynamicSnapshotFacility : AbstractFacility
    {
        private readonly Assembly _assemblyWithAggreagateRoots;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicSnapshotFacility"/> class.
        /// </summary>
        /// <param name="assemblyName">The assembly name with aggregate roots.</param>
        public DynamicSnapshotFacility(string assemblyName)
            : this(Assembly.Load(assemblyName))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicSnapshotFacility"/> class.
        /// </summary>
        /// <param name="assemblyWithAggregateRoots">The assembly with aggregate roots.</param>
        public DynamicSnapshotFacility(Assembly assemblyWithAggregateRoots)
        {
            _assemblyWithAggreagateRoots = assemblyWithAggregateRoots;
        }

        protected override void Init()
        {
            Kernel.Register(
                Component
                    .For<IAggregateRootCreationStrategy>()
                    .ImplementedBy<DynamicSnapshotAggregateRootCreationStrategy>(),
                Component
                    .For<IDynamicSnapshotAssembly>()
                    .ImplementedBy<DynamicSnapshotAssembly>()
                    .OnCreate((kernel, instance) => instance.CreateAssemblyFrom(_assemblyWithAggreagateRoots)),
                Component.For<SnapshotableAggregateRootFactory>(),
                Component.For<DynamicSnapshotAssemblyBuilder>(),
                Component.For<DynamicSnapshotTypeBuilder>(),
                Component.For<SnapshotableImplementerFactory>());
        }

    }
}

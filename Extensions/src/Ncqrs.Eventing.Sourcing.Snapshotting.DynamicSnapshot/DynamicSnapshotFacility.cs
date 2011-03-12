using System;
using Castle.MicroKernel.Facilities;
using Ncqrs.Domain.Storage;
using Castle.MicroKernel.Registration;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    public class DynamicSnapshotFacility : AbstractFacility
    {
        protected override void Init()
        {
            Kernel.Register(
                Component
                    .For<IAggregateRootCreationStrategy>()
                    .ImplementedBy<DynamicSnapshotAggregateRootCrationStrategy>()
                    .LifeStyle.Transient);
        }
    }
}

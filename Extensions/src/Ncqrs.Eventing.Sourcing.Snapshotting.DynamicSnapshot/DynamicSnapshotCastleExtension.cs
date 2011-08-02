using System;
using Ncqrs.Domain;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    public static class DynamicSnapshotCastleExtension
    {
        /// <summary>
        /// Registers the type as ISnapshotable in the container.
        /// </summary>
        public static ComponentRegistration<T> AsSnapshotable<T>(this ComponentRegistration<T> self) where T : AggregateRoot
        {
            return self
                .UsingFactoryMethod<T>((kernel, context) =>
                {
                    var factory = kernel.Resolve<SnapshotableAggregateRootFactory>();
                    return (T)factory.Create(typeof(T));
                });
        }
    }
}

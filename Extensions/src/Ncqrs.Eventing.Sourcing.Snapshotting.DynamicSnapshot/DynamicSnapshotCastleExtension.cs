using System;
using Ncqrs.Domain;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    public static class DynamicSnapshotCastleExtension
    {
        public static ComponentRegistration<T> AsSnapshotable<T>(this ComponentRegistration<T> self) where T : AggregateRoot
        {
            return self
                .Proxy.MixIns(DynamicSnapshot.CreateSnapshotable<T>())
                .OnCreate((kernel, instance) =>
                {
                    var proxy = instance as IHaveProxyReference;
                    if (proxy != null)
                        proxy.Proxy = instance;
                });
        }
    }
}

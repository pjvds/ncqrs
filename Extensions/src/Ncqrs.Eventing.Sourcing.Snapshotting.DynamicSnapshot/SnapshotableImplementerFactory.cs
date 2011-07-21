using System;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    internal class SnapshotableImplementerFactory
    {
        public SnapshotableImplementerFactory()
        {

        }

        public object Create(Type snapshotType)
        {
            if (!typeof(DynamicSnapshotBase).IsAssignableFrom(snapshotType))
                throw new ArgumentException("snapshotType must inherit DynamicSnapshotBase");

            var snapshotableImplementerType = typeof(SnapshotableImplementer<>).MakeGenericType(snapshotType);
            return Activator.CreateInstance(snapshotableImplementerType);
        }
    }
}

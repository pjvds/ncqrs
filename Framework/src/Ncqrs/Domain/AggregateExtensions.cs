using System;
using System.Linq;
using System.Reflection;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Microsoft.Extensions.Logging;

namespace Ncqrs.Domain
{
    internal static class AggregateRootExtensions
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(AggregateRootExtensions));

        public static Type GetSnapshotInterfaceType(this Type aggregateType)
        {
            // Query all ISnapshotable interfaces. We only allow only
            // one ISnapshotable interface per aggregate root type.
            var snapshotables = from i in aggregateType.GetInterfaces()
                                where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISnapshotable<>)
                                select i;

            // Aggregate does not implement any ISnapshotable interface.
            if (snapshotables.Count() == 0)
            {
                Log.LogDebug("No snapshot interface found on aggregate root {0}.", aggregateType.FullName);
                return null;
            }
            // Aggregate does implement multiple ISnapshotable interfaces.
            if (snapshotables.Count() > 1)
            {
                Log.LogWarning("Aggregate root {0} contains multiple snapshot interfaces while only one is allowed.", aggregateType.FullName);
                return null;
            }

            var snapshotableInterfaceType = snapshotables.Single();
            Log.LogDebug("Found snapshot interface {0} on aggregate root {1}.", snapshotableInterfaceType.FullName, aggregateType.FullName);

            return snapshotableInterfaceType;
        }
    }
}

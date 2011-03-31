using System;
using System.Linq;
using System.Reflection;
using Ncqrs.Eventing.Sourcing.Snapshotting;

namespace Ncqrs.Domain
{
    internal static class AggregateRootExtensions
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
                Log.DebugFormat("No snapshot interface found on aggregate root {0}.", aggregateType.FullName);
                return null;
            }
            // Aggregate does implement multiple ISnapshotable interfaces.
            if (snapshotables.Count() > 1)
            {
                Log.WarnFormat("Aggregate root {0} contains multiple snapshot interfaces while only one is allowed.", aggregateType.FullName);
                return null;
            }

            var snapshotableInterfaceType = snapshotables.Single();
            Log.DebugFormat("Found snapshot interface {0} on aggregate root {1}.", snapshotableInterfaceType.FullName, aggregateType.FullName);

            return snapshotableInterfaceType;
        }
    }
}

using System;
using System.Reflection;
using Ncqrs.Domain;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    /// <summary>
    /// Implements <see cref="ISnapshotable&lt;TSnapshot&gt;"/> interface.
    /// </summary>
    /// <typeparam name="TSnapshot">The type of the snapshot.</typeparam>
    internal class SnapshotableImplementer<TSnapshot> : ISnapshotableImplementer<TSnapshot>
            where TSnapshot : DynamicSnapshotBase
    {
        public object Proxy { private get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotableImplementer&lt;TSnapshot&gt;"/> class.
        /// </summary>
        public SnapshotableImplementer()
        {

        }

        public TSnapshot CreateSnapshot()
        {
            var snapshot = (TSnapshot)Activator.CreateInstance(typeof(TSnapshot));
            TransferState(snapshot, (AggregateRoot)Proxy, TransferDirection.ToSnapshot);
            return snapshot;
        }

        public void RestoreFromSnapshot(TSnapshot snapshot)
        {
            TransferState(snapshot, (AggregateRoot)Proxy, TransferDirection.ToAggregateRoot);
        }

        private enum TransferDirection
        {
            ToSnapshot,
            ToAggregateRoot
        }

        private void TransferState(DynamicSnapshotBase snapshot, AggregateRoot aggregate, TransferDirection direction)
        {
            if (snapshot == null) throw new ArgumentNullException("snapshot");
            if (aggregate == null) throw new ArgumentNullException("source");

            var aggregateFieldMap = SnapshotableField.GetMap(aggregate.GetType());
            var snapshotFields = snapshot.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            Action<object, FieldInfo, object, FieldInfo> doTransfer = null;

            if (direction == TransferDirection.ToAggregateRoot)
            {
                doTransfer = (source, sourceField, destination, destinationField)
                    => destinationField.SetValue(destination, sourceField.GetValue(source));
            }
            else
            {
                doTransfer = (destination, destinationField, source, sourceField)
                    => destinationField.SetValue(destination, sourceField.GetValue(source));
            }

            foreach (var snapshotField in snapshotFields)
            {
                FieldInfo aggregateField;
                if (aggregateFieldMap.TryGetValue(snapshotField.Name, out aggregateField))
                {
                    doTransfer(snapshot, snapshotField, aggregate, aggregateField);
                }
                else
                {
                    // TODO: No field found; throw?
                }
            }
        }

    }
}

namespace Ncqrs.Eventing.Storage.SQLite {
    using System;
    using Ncqrs.Eventing.Sourcing.Snapshotting;

    public class SQLiteSnapshotStore :ISnapshotStore{
        public void SaveShapshot(ISnapshot source)
        {
            throw new NotImplementedException();
        }

        public ISnapshot GetSnapshot(Guid eventSourceId)
        {
            throw new NotImplementedException();
        }
    }
}

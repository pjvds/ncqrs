using System;

namespace Ncqrs.SerializableSnapshots
{
    public interface ISerializableSnapshotStore
    {
        object GetSnapshot(Guid id);
        void SaveSnapshot(object aggreate);
    }
}
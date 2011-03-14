using System;

namespace Ncqrs.Eventing.Storage.Postgre
{
    internal static class Queries
    {
        public const String DeleteUnusedProviders = "DELETE FROM EventSources WHERE (SELECT Count(EventSourceId) FROM Events WHERE EventSourceId=EventSources.Id) = 0";

        public const String InsertNewEventQuery = "INSERT INTO Events(Id, EventSourceId, Name, Version, Data, Sequence, TimeStamp) VALUES (:EventId, :EventSourceId, :Name, :Version, :Data, :Sequence, :TimeStamp)";

        public const String InsertNewProviderQuery = "INSERT INTO EventSources(Id, Type, Version) VALUES (:Id, :Type, :Version)";

        public const String SelectAllEventsQuery = "SELECT Id, EventSourceId, Name, Version, TimeStamp, Data, Sequence FROM Events WHERE EventSourceId = :EventSourceId AND Sequence >= :EventSourceMinVersion AND Sequence <= :EventSourceMaxVersion ORDER BY Sequence";

        public const String SelectEventsAfterQuery = "SELECT TOP {0} Id, EventSourceId, Name, Version, TimeStamp, Data, Sequence FROM Events WHERE SequentialId > (SELECT SequentialId FROM Events WHERE Id = :EventId) ORDER BY SequentialId";

        public const String SelectEventsFromBeginningOfTime = "SELECT TOP {0} Id, EventSourceId, Name, Version, TimeStamp, Data, Sequence FROM Events ORDER BY SequentialId";

        public const String SelectAllIdsForTypeQuery = "SELECT Id FROM EventSources WHERE Type = :Type";

        public const String SelectVersionQuery = "SELECT Version FROM EventSources WHERE Id = :id";

        public const String UpdateEventSourceVersionQuery = "UPDATE EventSources SET Version = :NewVersion WHERE Id = :id";

        public const String InsertSnapshot = "DELETE FROM Snapshots WHERE EventSourceId=:EventSourceId; INSERT INTO Snapshots(EventSourceId, Timestamp, Version, Type, Data) VALUES (:EventSourceId, GETDATE(), :Version, :Type, :Data)";

        public const String SelectLatestSnapshot = "SELECT TOP 1 * FROM Snapshots WHERE EventSourceId=:EventSourceId ORDER BY Version DESC";
    }
}

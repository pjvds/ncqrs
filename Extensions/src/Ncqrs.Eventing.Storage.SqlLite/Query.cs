namespace Ncqrs.Eventing.Storage.SQLite {
    //"copy paste code inheritance" from Ncqrs.Eventing.Storage.SQL.SimpleMicrosoftSqlServerEventStore
    internal class Query {
        internal const string DeleteUnusedProviders="DELETE FROM [EventSources] WHERE (SELECT Count(EventSourceId) FROM [Events] WHERE [EventSourceId]=[EventSources].[Id]) = 0";

        internal const string InsertNewEventQuery="INSERT INTO [Events]([EventSourceId], [EventId], [Name], [Data], [Sequence], [TimeStamp]) VALUES (@SourceId, @EventId, @Name, @Data, @Sequence, @Timestamp)";

        internal const string InsertNewProviderQuery="INSERT INTO [EventSources](Id, Type, Version) VALUES (@Id, @Type, @Version)";

        internal const string SelectAllEventsFromQuery = "SELECT [TimeStamp], [EventId], [Data], [Sequence] FROM [Events] WHERE [EventSourceId] = @EventSourceId AND [Sequence] >= @EventSourceMinVersion AND [Sequence] <= @EventSourceMaxVersion ORDER BY [Sequence]";

        internal const string SelectAllIdsForTypeQuery="SELECT [Id] FROM [EventSources] WHERE [Type] = @Type";

        internal const string SelectVersionQuery="SELECT [Version] FROM [EventSources] WHERE [Id] = @id";

        internal const string UpdateEventSourceVersionQuery="UPDATE EventSources SET Version = (SELECT Count(*) FROM Events WHERE EventSourceId = @Id) WHERE Id = @Id";

        internal const string InsertSnapshot="DELETE FROM [Snapshots] WHERE [EventSourceId]=@EventSourceId; INSERT INTO [Snapshots]([EventSourceId], [Version], [SnapshotType], [SnapshotData], [TimeStamp]) VALUES (@EventSourceId, @Version, @SnapshotType, @SnapshotData, @Timestamp)";

        internal const string SelectLatestSnapshot="SELECT TOP 1 * FROM [Snapshots] WHERE [EventSourceId]=@EventSourceId ORDER BY Version DESC";

        internal const string CreateTables=
                      @"CREATE TABLE [Events] (
                            [EventSourceId] GUID  NOT NULL,
                            [EventId] GUID  NOT NULL,
                            [Sequence] INTEGER  NOT NULL,
                            [TimeStamp] INTEGER NOT NULL,
                            [Data] BLOB  NOT NULL,
                            [Name] nvarchar(255)  NOT NULL);

                        CREATE TABLE EventSources(Id GUID UNIQUE NOT NULL,
                                                  Type nvarchar(255) NOT NULL,
                                                  Version int NOT NULL);

                        CREATE TABLE Snapshots (EventSourceId GUID UNIQUE NOT NULL,
                                                 Version INTEGER,
                                                 TimeStamp INTEGER NOT NULL,
                                                 Data BLOB NOT NULL);";
    }
}

CREATE TABLE 
	[dbo].[Events]
	(
		[Id] [uniqueidentifier] NOT NULL, [EventSourceId] [uniqueidentifier] NOT NULL, [Sequence] [bigint], 
		[TimeStamp] [datetime] NOT NULL, [Data] [varbinary](max) NOT NULL, [Name] [varchar](max) NOT NULL
	)
	ON [PRIMARY];
	GO;
CREATE TABLE [dbo].[EventSources]([Id] [uniqueidentifier] NOT NULL, [Type] [nvarchar](255) NOT NULL, [Version] [int] NOT NULL) ON [PRIMARY];
CREATE TABLE [dbo].[Snaptshots]([EventSourceId] [uniqueidentifier] NOT NuLL, [Version] [bigint], [TimeStamp] [datetime] NOt NULL, [Data] [varbinary](max) NOT NULL) ON [PRIMARY]
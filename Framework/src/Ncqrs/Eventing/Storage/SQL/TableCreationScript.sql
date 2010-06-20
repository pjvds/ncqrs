CREATE TABLE [dbo].[Events]
(
	[Id] [uniqueidentifier] NOT NULL, [EventSourceId] [uniqueidentifier] NOT NULL, [Sequence] [bigint], 
	[TimeStamp] [datetime] NOT NULL, [Data] [varbinary](max) NOT NULL, [Name] [varchar](max) NOT NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[EventSources]
(
	[Id] [uniqueidentifier] NOT NULL, [Type] [nvarchar](255) NOT NULL, [Version] [int] NOT NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Snapshots]
(
	[EventSourceId] [uniqueidentifier] NOT NULL, [Version] [bigint], [TimeStamp] [datetime] NOT NULL, 
	[Type] varchar(255) NOT NULL, [Data] [varbinary](max) NOT NULL
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[Events]
(
	[SequentialId] [int] NOT NULL IDENTITY (1, 1)
	[Id] [uniqueidentifier] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,

	[Name] [varchar](max) NOT NULL,
	[Version] [varchar](max) NOT NULL,

	[EventSourceId] [uniqueidentifier] NOT NULL,
	[Sequence] [bigint], 

	[Data] [nvarchar](max) NOT NULL
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
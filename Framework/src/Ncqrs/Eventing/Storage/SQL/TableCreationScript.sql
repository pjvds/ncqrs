IF EXISTS(SELECT * FROM sysobjects WHERE name='Events' AND xtype = 'U') RETURN;

CREATE TABLE [dbo].[Events](
	[SequentialId] [int] IDENTITY(1,1) NOT NULL,
	[Id] [uniqueidentifier] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[Name] [varchar](max) NOT NULL,
	[Version] [varchar](max) NOT NULL,
	[EventSourceId] [uniqueidentifier] NOT NULL,
	[Sequence] [bigint] NULL,
	[Data] [nvarchar](max) NOT NULL,
	CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED 
	(
		[SequentialId] ASC
	)
	WITH
	(
		PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF,
		IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON,
		ALLOW_PAGE_LOCKS  = ON
	)
) ON [PRIMARY]

CREATE TABLE [dbo].[EventSources]
(
	[Id] [uniqueidentifier] NOT NULL, [Type] [nvarchar](255) NOT NULL, [Version] [int] NOT NULL
) ON [PRIMARY]


CREATE UNIQUE NONCLUSTERED INDEX [IX_Id] ON [dbo].[EventSources] 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

CREATE TABLE [dbo].[Snapshots]
(
	[EventSourceId] [uniqueidentifier] NOT NULL, [Version] [bigint], [TimeStamp] [datetime] NOT NULL, 
	[Type] varchar(255) NOT NULL, [Data] [varbinary](max) NOT NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[PipelineState](
	[BatchId] [int] IDENTITY(1,1) NOT NULL,
	[PipelineName] [varchar](255) NOT NULL,
	[LastProcessedEventId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_MainPipelineState] PRIMARY KEY CLUSTERED 
	(
		[BatchId] ASC
	)
	WITH
	(
		PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF,
		IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, 
		ALLOW_PAGE_LOCKS  = ON
	)
) ON [PRIMARY]

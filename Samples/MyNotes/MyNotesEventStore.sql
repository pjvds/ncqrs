USE [master]
GO

IF EXISTS(SELECT * FROM sys.databases WHERE name = 'MyNotesEventStore')
	BEGIN
		DROP DATABASE [MyNotesEventStore]
	END
GO

CREATE DATABASE [MyNotesEventStore]
GO

USE [MyNotesEventStore]
GO

CREATE TABLE [EventSources]
(
	[Id]					[uniqueidentifier]		NOT NULL,
	[Type]					[nvarchar](255)			NOT NULL,
	[Version]				[int]					NOT NULL
) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Id] ON [EventSources] 
(
	[Id] ASC
)
WITH
(
	PAD_INDEX					= OFF,
	STATISTICS_NORECOMPUTE		= OFF,
	SORT_IN_TEMPDB				= OFF,
	IGNORE_DUP_KEY				= OFF,
	DROP_EXISTING				= OFF,
	ONLINE						= OFF,
	ALLOW_ROW_LOCKS				= ON,
	ALLOW_PAGE_LOCKS			= ON
) ON [PRIMARY]
GO

CREATE TABLE [Events]
(
	[SequentialId]			[int] IDENTITY(1,1)		NOT NULL,
	[Id]					[uniqueidentifier]		NOT NULL,
	[TimeStamp]				[datetime]				NOT NULL,
	[Name]					[varchar](max)			NOT NULL,
	[Version]				[varchar](max)			NOT NULL,
	[EventSourceId]			[uniqueidentifier]		NOT NULL,
	[Sequence]				[bigint]				NULL,
	[Data]					[nvarchar](max)			NOT NULL,
	CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED
	(
		[SequentialId] ASC
	)
	WITH
	(
		PAD_INDEX				= OFF,
		STATISTICS_NORECOMPUTE	= OFF,
		IGNORE_DUP_KEY			= OFF,
		ALLOW_ROW_LOCKS			= ON,
		ALLOW_PAGE_LOCKS		= ON
	)
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_EventSourceId] ON [Events] (EventSourceId)
GO

CREATE TABLE [Snapshots]
(
	[EventSourceId]			[uniqueidentifier]		NOT NULL,
	[Version]				[bigint]				NULL,
	[TimeStamp]				[datetime]				NOT NULL, 
	[Type]					varchar(255)			NOT NULL,
	[Data]					[varbinary](max)		NOT NULL
) ON [PRIMARY]
GO

CREATE TABLE [PipelineState]
(
	[BatchId]				[int] IDENTITY(1,1)		NOT NULL,
	[PipelineName]			[varchar](255)			NOT NULL,
	[LastProcessedEventId]	[uniqueidentifier]		NOT NULL,
	CONSTRAINT [PK_MainPipelineState] PRIMARY KEY CLUSTERED 
	(
		[BatchId] ASC
	)
	WITH
	(
		PAD_INDEX				= OFF,
		STATISTICS_NORECOMPUTE	= OFF,
		IGNORE_DUP_KEY			= OFF,
		ALLOW_ROW_LOCKS			= ON, 
		ALLOW_PAGE_LOCKS		= ON
	)
) ON [PRIMARY]
GO

ALTER TABLE [Events] ADD CONSTRAINT [UQ_Events_Id] UNIQUE ([Id])
GO

ALTER TABLE [Events] WITH CHECK ADD CONSTRAINT [FK_Events_EventSources] FOREIGN KEY([EventSourceId])
	REFERENCES [EventSources] ([Id])
GO

ALTER TABLE [Events] CHECK CONSTRAINT [FK_Events_EventSources]
GO

ALTER TABLE [Snapshots] WITH CHECK ADD CONSTRAINT [FK_Snapshots_EventSources] FOREIGN KEY([EventSourceId])
	REFERENCES [EventSources] ([Id])
GO

ALTER TABLE [Snapshots] CHECK CONSTRAINT [FK_Snapshots_EventSources]
GO

ALTER TABLE [PipelineState] WITH CHECK ADD CONSTRAINT [FK_PipelineState_Events] FOREIGN KEY([LastProcessedEventId])
	REFERENCES [Events] ([Id])
GO

ALTER TABLE [PipelineState] CHECK CONSTRAINT [FK_PipelineState_Events]
GO
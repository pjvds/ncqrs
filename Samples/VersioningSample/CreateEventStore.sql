USE [master]
GO

IF EXISTS(SELECT name FROM sys.databases WHERE name = 'VersioningEventStore')
	DROP DATABASE [VersioningEventStore]
GO

CREATE DATABASE [VersioningEventStore]
GO

USE [VersioningEventStore]

CREATE TABLE [dbo].[Events]
(
	[Id] [uniqueidentifier] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,

	[Name] [varchar](max) NOT NULL,
	[Version] [varchar](max) NOT NULL,

	[EventSourceId] [uniqueidentifier] NOT NULL,
	[Sequence] [bigint], 

	[Data] [varchar](max) NOT NULL 
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
USE [master]
GO

IF exists(SELECT * FROM sys.databases WHERE name = 'MyNotesEventStore')
BEGIN
	DROP DATABASE [MyNotesEventStore]
END
GO

CREATE DATABASE [MyNotesEventStore]
GO

USE [MyNotesEventStore]
GO

CREATE TABLE [dbo].[EventSources](
	[Id] [uniqueidentifier] PRIMARY KEY,
	[Type] [nvarchar](255) NOT NULL,
	[Version] [int] NOT NULL
) ON [PRIMARY]

CREATE TABLE [dbo].[Events](
	[Id] [uniqueidentifier] PRIMARY KEY,
	[EventSourceId] [uniqueidentifier] NOT NULL,
	[Sequence] [bigint] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[Data] [varbinary](max) NOT NULL,
	[Name] [varchar](max) NOT NULL
) ON [PRIMARY]
GO
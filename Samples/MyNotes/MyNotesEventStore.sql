USE [master]
GO

IF NOT exists(SELECT * FROM sys.databases WHERE name = N'MyNotesEventStore')
BEGIN
    CREATE DATABASE [MyNotesEventStore]
END

USE [MyNotesEventStore]
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EventSources]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[EventSources](
	[Id] [uniqueidentifier] NOT NULL,
	[Type] [nvarchar](255) NOT NULL,
	[Version] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Events]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Events](
	[Id] [uniqueidentifier] NOT NULL,
	[EventSourceId] [uniqueidentifier] NOT NULL,
	[Sequence] [bigint] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[Data] [varbinary](max) NOT NULL,
	[Name] [varchar](max) NOT NULL,
 CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
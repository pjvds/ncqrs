USE [master]
GO

IF NOT exists(SELECT * FROM sys.databases WHERE name = N'MyNotesReadModel')
BEGIN
    CREATE DATABASE [MyNotesReadModel]
END

USE [MyNotesReadModel]
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NoteItem]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[NoteItem](
		[Id] [uniqueidentifier] NOT NULL,
		[Text] [varchar](250) NULL,
		[CreationDate] [datetime] NULL,
	 CONSTRAINT [PK_NoteItem] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

SET ANSI_PADDING OFF
GO
